using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

enum IPCCommand : byte
{
    MsgRead8 = 0,           /**< Read 8 bit value to memory. */
    MsgRead16 = 1,          /**< Read 16 bit value to memory. */
    MsgRead32 = 2,          /**< Read 32 bit value to memory. */
    MsgRead64 = 3,          /**< Read 64 bit value to memory. */
    MsgWrite8 = 4,          /**< Write 8 bit value to memory. */
    MsgWrite16 = 5,         /**< Write 16 bit value to memory. */
    MsgWrite32 = 6,         /**< Write 32 bit value to memory. */
    MsgWrite64 = 7,         /**< Write 64 bit value to memory. */
    MsgVersion = 8,         /**< Returns the emulator version. */
    MsgSaveState = 9,       /**< Saves a savestate. */
    MsgLoadState = 0xA,     /**< Loads a savestate. */
    MsgTitle = 0xB,         /**< Returns the game title. */
    MsgID = 0xC,            /**< Returns the game ID. */
    MsgUUID = 0xD,          /**< Returns the game UUID. */
    MsgGameVersion = 0xE,   /**< Returns the game verion. */
    MsgStatus = 0xF,        /**< Returns the emulator status. */

    MsgReadN = 100,         /** */
    MsgWriteN = 101,        /** */
    MsgFrameAdvance = 102,  /** */
    MsgResume = 103,        /** */
    MsgPause = 104,        /** */
    MsgRestart = 105,        /** */
    MsgStop = 106,        /** */
    MsgGetFrameBuffer = 107,   /** */
    MsgSetPad = 108,   /** */
    MsgGetVmPtr = 109, /** */
    MsgSetDynamicSetting = 110, /** */

    MsgUnimplemented = 0xFF /**< Unimplemented IPC message. */
};

public enum DynamicSettingId : byte
{
    DynamicSettingFrameSleepWait = 0,
    DynamicSettingDisableRendering = 1,
}

public enum IPCStatusCode : uint
{
    Running = 0,
    Paused = 1,
    Shutdown = 2
}

public struct IPCStatus
{
    public IPCStatusCode Code;
    public uint Frame;
};

public struct EmuMemoryOffsets
{
    public ulong RealtimeMemoryOffset;
    public ulong BackbufferMemoryOffset;
}

public class PINE
{
    public struct Promise<T>
    {
        public static readonly Promise<T> Empty = new Promise<T>(null, null);

        private PINE _pine;
        private Request _request;
        private Func<object, T> _parser;
        private T _result;
        private bool _hasLocalResult;

        public bool TryGetResult(out T result)
        {
            if (_hasLocalResult)
            {
                result = _result;
                return result != null;
            }

            result = default(T);
            if (_request == null || _pine == null)
                return false;

            if (_request.HasResult)
            {
                result = _parser(_request.Result);
                return result != null;
            }

            _pine.SendCommandsAndWait();

            if (_request.Result == null)
                return false;

            result = _parser(_request.Result);
            return _request.HasResult && result != null;
        }

        internal Promise(PINE pine, Request request, Func<object, T> parser = null)
        {
            _pine = pine;
            _request = request;
            _parser = parser;
            _result = default(T);
            _hasLocalResult = false;

            if (_parser == null)
                _parser = DefaultParser;
        }

        internal Promise(T result)
        {
            _pine = null;
            _request = null;
            _parser = null;
            _result = result;
            _hasLocalResult = true;
        }

        T DefaultParser(object result)
        {
            return (T)result;
        }

        public static Promise<T> TransformPromise<T, TFrom>(Promise<TFrom> promise, Func<TFrom, T> parser = null)
        {
            var transformed = new Promise<T>(promise._pine, promise._request);
            transformed._parser = (d) => { return parser == null ? transformed.DefaultParser(d) : parser((TFrom)d); };

            if (promise._hasLocalResult)
            {
                transformed._hasLocalResult = true;
                transformed._result = transformed._parser(promise._result);
            }

            return transformed;
        }
    }

    public int Port { get; private set; }

    TcpClient client = null;
    private bool connectFailed = false;
    Queue<Request> commandQueue = new Queue<Request>();

    byte[] commandBuffer = new byte[0x1000000];
    MemoryStream commandBufferStream;
    BinaryWriter commandBufferWriter;
    private readonly object commandLock = new object();

    public bool Connected => client?.Connected ?? false;
    public bool Connecting => !Connected && !connectFailed;
    public bool ConnectFailed => connectFailed;

    public PINE(int port)
    {
        Port = port;
        commandBufferStream = new MemoryStream(commandBuffer);
        commandBufferWriter = new BinaryWriter(commandBufferStream);

        client = new TcpClient()
        {
            ReceiveBufferSize = commandBuffer.Length,
            SendBufferSize = commandBuffer.Length,
            ReceiveTimeout = 100000,
            SendTimeout = 100000,
            NoDelay = true
        };

        client.ConnectAsync("127.0.0.1", port).ContinueWith((t) =>
        {
            connectFailed = !client.Connected;
            if (client.Connected)
                Debug.Log("client connected");
        });

        //client = new TcpClient("127.0.0.1", port)
        //{
        //    ReceiveBufferSize = commandBuffer.Length,
        //    SendBufferSize = commandBuffer.Length,
        //    ReceiveTimeout = 1000,
        //    SendTimeout = 1000,
        //    NoDelay = true
        //};

    }

    ~PINE()
    {
        Close();

        if (commandBufferWriter != null)
        {
            commandBufferWriter.Dispose();
            commandBufferWriter = null;
        }

        if (commandBufferStream != null)
        {
            commandBufferStream.Dispose();
            commandBufferStream = null;
        }

        commandBuffer = null;
    }

    public void Close()
    {
        if (Connected)
        {
            client.Close();
            client.Dispose();
            client = null;
        }
    }

    private void Receiver()
    {
        try
        {
            var frameBuffer = new byte[0x10000];
            var rollingBuffer = new byte[0x10000];
            var rollingBufferStart = 0;
            var bytes = new byte[client.ReceiveBufferSize];
            NetworkStream ns = client.GetStream();
            using (var ms = new MemoryStream(rollingBuffer, true))
            {
                using (var bw = new BinaryWriter(ms))
                {
                    while (true)
                    {
                        if (client.ReceiveBufferSize > 0 && ns.CanRead)
                        {
                            int read = ns.Read(bytes, 0, client.ReceiveBufferSize);
                            if (read > 0)
                            {
                                lock (commandLock)
                                {
                                    // write into buffer
                                    var newEnd = ms.Position + read;
                                    if (newEnd > rollingBuffer.Length)
                                    {
                                        int segmentLen = (int)(rollingBuffer.Length - ms.Position);
                                        bw.Write(bytes, 0, segmentLen);
                                        ms.Position = 0;
                                        bw.Write(bytes, segmentLen, read - segmentLen);
                                    }
                                    else
                                    {
                                        bw.Write(bytes, 0, read);
                                    }

                                    // try and parse frame
                                    int rollingBufferSize = (ms.Position < rollingBufferStart) ? (int)((rollingBuffer.Length - rollingBufferStart) + ms.Position) : (int)(ms.Position - rollingBufferStart);
                                    int frameLen = BitConverter.ToInt32(rollingBuffer, rollingBufferStart);
                                    if (frameLen >= rollingBufferSize)
                                    {
                                        // copy to buffer
                                        if (ms.Position < rollingBufferStart)
                                        {
                                            Array.Copy(rollingBuffer, rollingBufferStart, frameBuffer, 0, rollingBuffer.Length - rollingBufferStart);
                                            Array.Copy(rollingBuffer, 0, frameBuffer, rollingBuffer.Length - rollingBufferStart, ms.Position);
                                        }
                                        else
                                        {
                                            Array.Copy(rollingBuffer, rollingBufferStart, frameBuffer, 0, rollingBufferSize);
                                        }

                                        // parse
                                        ProcessFrame(frameBuffer, 0, frameLen);

                                        // pop frame
                                        rollingBufferStart = (rollingBufferStart + frameLen) % rollingBuffer.Length;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            //e
        }
    }

    private void ProcessFrame(byte[] buffer, int offset, int length)
    {
        using (var ms = new MemoryStream(buffer, offset, length, false))
        {
            using (var reader = new BinaryReader(ms))
            {
                reader.ReadInt32(); // frame length
                byte responseCode = reader.ReadByte();

                if (responseCode == 0)
                {
                    while (ms.Position < ms.Length)
                    {
                        if (commandQueue.TryDequeue(out var request))
                        {
                            request.Read(reader);
                        }
                    }

                    while (commandQueue.TryDequeue(out var request))
                        request.Read(reader);
                }
                else
                {
                    while (commandQueue.TryDequeue(out var request))
                        request.Failed();
                }
            }
        }
    }

    public Promise<byte[]> ReadBytes(long address, long length)
    {
        if (!Connected)
            return Promise<byte[]>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_ReadN((uint)address, (int)length) { Id = id };
        var promise = new Promise<byte[]>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public bool WriteBytes(long address, byte[] data, int offset, int length)
    {
        if (!Connected)
            return false;

        // batch large writes
        int totalLength = length;
        int sent = 0;

        while (sent < totalLength)
        {
            var blockSize = length - sent;
            if (blockSize > 0x8000)
                blockSize = 0x8000;

            if ((commandBufferWriter.BaseStream.Position + blockSize) > 0x10000)
                SendCommandsAndWait();

            // add command to queue
            var request = new Request_WriteN((uint)(address + sent), data, offset + sent, blockSize) { Id = Guid.NewGuid() };
            commandQueue.Enqueue(request);
            request.Write(commandBufferWriter);

            sent += blockSize;
        }

        return true;
    }

    public Promise<bool> FrameAdvance()
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_FrameAdvance() { Id = id };
        var promise = new Promise<bool>(this, request, (data) => data is bool b && b);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> Resume()
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_Resume() { Id = id };
        var promise = new Promise<bool>(this, request, (data) => data is bool b && b);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> Pause()
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_Pause() { Id = id };
        var promise = new Promise<bool>(this, request, (data) => data is bool b && b);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> Restart()
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_Restart() { Id = id };
        var promise = new Promise<bool>(this, request, (data) => data is bool b && b);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> Stop()
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_Stop() { Id = id };
        var promise = new Promise<bool>(this, request, (data) => data is bool b && b);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> LoadSaveState(int stateIdx)
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_LoadSaveState() { Id = id, StateIdx = stateIdx };
        var promise = new Promise<bool>(this, request, (data) => data is bool b && b);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<IPCStatus> GetStatus()
    {
        if (!Connected)
            return Promise<IPCStatus>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_GetStatus() { Id = id };
        var promise = new Promise<IPCStatus>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> GetFrameBuffer(byte[] buffer)
    {
        if (!Connected || buffer == null || buffer.Length != (512*448*4))
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_GetFrameBuffer(buffer) { Id = id };
        var promise = new Promise<bool>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> SetPad(int port, int slot, byte[] padData)
    {
        if (!Connected || padData == null || padData.Length != 32)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_SetPad(port, slot, padData) { Id = id };
        var promise = new Promise<bool>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<EmuMemoryOffsets> GetVmPtr()
    {
        if (!Connected)
            return Promise<EmuMemoryOffsets>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_GetVmPtr() { Id = id };
        var promise = new Promise<EmuMemoryOffsets>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> SetFrameSleepWait(bool sleepWhileWaiting)
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_SetFrameSleepWait(sleepWhileWaiting) { Id = id };
        var promise = new Promise<bool>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public Promise<bool> SetDisableRendering(bool disableRendering)
    {
        if (!Connected)
            return Promise<bool>.Empty;

        // create promise
        var id = Guid.NewGuid();
        var request = new Request_SetDisableRendering(disableRendering) { Id = id };
        var promise = new Promise<bool>(this, request);

        // add command to queue
        commandQueue.Enqueue(request);

        // write command
        request.Write(commandBufferWriter);

        return promise;
    }

    public void SendCommandsAndWait(int maxWaitMs = 5)
    {
        if (!Connected)
            return;

        if (commandBufferStream.Position > 0)
        {
            lock (commandLock)
            {
                // send
                client.GetStream().Write(BitConverter.GetBytes(4 + (int)commandBufferStream.Position));
                client.GetStream().Write(commandBuffer, 0, (int)commandBufferStream.Position);
                client.GetStream().Flush();
                commandBufferStream.Position = 0;
            }
        }

        int offset = 0;
        while (commandQueue.Count > 0 && client.GetStream().CanRead)
        {
            int read = client.GetStream().Read(commandBuffer, offset, client.ReceiveBufferSize - offset);
            if (read > 0)
            {
                int totalRead = offset + read;
                int frameLen = BitConverter.ToInt32(commandBuffer, 0);
                if (frameLen > totalRead)
                {
                    offset += read;
                    continue;
                    //commandQueue.Clear();
                    //throw new Exception("corruption.");
                }

                if (frameLen != totalRead)
                {
                    Debug.Log($"frame len is {frameLen} but read {read}... we should probably write some to handle fragmented frames");
                }

                // parse
                ProcessFrame(commandBuffer, 0, frameLen);
            }
        }
    }

    bool isMemoryAddressValid(long address)
    {
        return address >= 0x00080000 && address < 0x02000000;
    }
}


internal abstract class Request
{
    public DateTime UtcTimeSent = DateTime.UtcNow;
    public Guid Id { get; set; }
    public abstract IPCCommand Command { get; }
    public object Result { get; set; }
    public bool HasResult { get; set; }

    public abstract void Write(BinaryWriter writer);
    public abstract void Read(BinaryReader reader);
    public abstract void Failed();
}

internal class Request_ReadN : Request
{
    public override IPCCommand Command => IPCCommand.MsgReadN;
    public uint Address { get; set; }
    public int Length { get; set; }

    public Request_ReadN(uint address, int length)
    {
        if (length > 0x8000)
            throw new ArgumentException($"PINE ReadN length must be no greater than {0x8000}");

        Address = address;
        Length = length;
    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBytes((ushort)Length);
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
        writer.Write((uint)Address);
        writer.Write((ushort)Length);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_WriteN : Request
{
    public override IPCCommand Command => IPCCommand.MsgWriteN;
    public uint Address { get; set; }
    public byte[] Data { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }

    public Request_WriteN(uint address, byte[] data, int offset, int length)
    {
        if (length > 0x8000)
            throw new ArgumentException($"PINE WriteN length must be no greater than {0x8000}");

        Address = address;
        Data = data;
        Offset = offset;
        Length = length;
    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
        writer.Write((uint)Address);
        writer.Write((ushort)Length);
        writer.Write(Data, Offset, Length);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_FrameAdvance : Request
{
    public override IPCCommand Command => IPCCommand.MsgFrameAdvance;

    public Request_FrameAdvance()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_Resume : Request
{
    public override IPCCommand Command => IPCCommand.MsgResume;

    public Request_Resume()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_Pause : Request
{
    public override IPCCommand Command => IPCCommand.MsgPause;

    public Request_Pause()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_Restart : Request
{
    public override IPCCommand Command => IPCCommand.MsgRestart;

    public Request_Restart()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_Stop : Request
{
    public override IPCCommand Command => IPCCommand.MsgStop;

    public Request_Stop()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_LoadSaveState : Request
{
    public override IPCCommand Command => IPCCommand.MsgLoadState;
    public int StateIdx { get; set; }

    public Request_LoadSaveState()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = true;
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
        writer.Write((byte)StateIdx);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_GetStatus : Request
{
    public override IPCCommand Command => IPCCommand.MsgStatus;

    public Request_GetStatus()
    {

    }

    public override void Read(BinaryReader reader)
    {
        Result = new IPCStatus() { Code = (IPCStatusCode)reader.ReadInt32(), Frame = reader.ReadUInt32() };
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_GetFrameBuffer : Request
{
    public override IPCCommand Command => IPCCommand.MsgGetFrameBuffer;

    public byte[] Buffer { get; }

    public Request_GetFrameBuffer(byte[] buffer)
    {
        Buffer = buffer;
    }

    public override void Read(BinaryReader reader)
    {
        reader.BaseStream.Read(Buffer, 0, 512 * 448 * 4);
        Result = true;
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = false;
    }
}

internal class Request_SetPad : Request
{
    public override IPCCommand Command => IPCCommand.MsgSetPad;
    public int Port { get; }
    public int Slot { get; }
    public byte[] PadData { get; }

    public Request_SetPad(int port, int slot, byte[] padData)
    {
        if (padData == null || padData.Length != 32)
            throw new ArgumentException($"PINE PadData length must be exactly 32!");

        Port = port;
        Slot = slot;
        PadData = padData;
    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
        writer.Write((short)Port);
        writer.Write((short)Slot);
        writer.Write(PadData, 0, 32);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_GetVmPtr : Request
{
    public override IPCCommand Command => IPCCommand.MsgGetVmPtr;

    public Request_GetVmPtr()
    {

    }

    public override void Read(BinaryReader reader)
    {
        ulong realtimeOffset = reader.ReadUInt64();
        ulong backbufferOffset = reader.ReadUInt64();

        Result = new EmuMemoryOffsets()
        {
            RealtimeMemoryOffset = realtimeOffset,
            BackbufferMemoryOffset = backbufferOffset
        };
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal abstract class Request_SetDynamicSetting : Request
{
    public override IPCCommand Command => IPCCommand.MsgSetDynamicSetting;
    public DynamicSettingId SettingId { get; set; }

    public Request_SetDynamicSetting(DynamicSettingId settingId)
    {
        SettingId = settingId;
    }

    public override void Read(BinaryReader reader)
    {
        Result = reader.ReadBoolean();
        HasResult = true;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write((byte)Command);
        writer.Write((byte)SettingId);
    }

    public override void Failed()
    {
        HasResult = true;
        Result = null;
    }
}

internal class Request_SetFrameSleepWait : Request_SetDynamicSetting
{
    public bool SleepWhileWaiting { get; set; }

    public Request_SetFrameSleepWait(bool sleepWhileWaiting) : base(DynamicSettingId.DynamicSettingFrameSleepWait)
    {
        SleepWhileWaiting = sleepWhileWaiting;
    }

    public override void Write(BinaryWriter writer)
    {
        base.Write(writer);
        writer.Write((byte)(SleepWhileWaiting ? 1 : 0));
    }
}

internal class Request_SetDisableRendering : Request_SetDynamicSetting
{
    public bool DisableRendering { get; set; }

    public Request_SetDisableRendering(bool disableRendering) : base(DynamicSettingId.DynamicSettingDisableRendering)
    {
        DisableRendering = disableRendering;
    }

    public override void Write(BinaryWriter writer)
    {
        base.Write(writer);
        writer.Write((byte)(DisableRendering ? 1 : 0));
    }
}
