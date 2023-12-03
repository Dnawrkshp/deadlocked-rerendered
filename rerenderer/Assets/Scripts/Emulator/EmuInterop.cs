using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using RC;

public static class EmuInterop
{
    static readonly long EMU_MEMORY_SIZE = 0x02200000;
    static readonly uint CACHE_BLOCK_SIZE = 0x8000;

    static ThreadLocal<PINE> _pine = new ThreadLocal<PINE>();
    static Thread _bgThread = null;
    static ConcurrentDictionary<uint, MemoryCache> _memoryCache = new ConcurrentDictionary<uint, MemoryCache>();
    static bool _tickQueued = true;
    static uint _lastFrame = 0;
    static double _lastFrameMs = 0;
    static int _pinePort = 28011;
    static uint? _emuProcessId = null;
    static ProcessMemory _emuMem = null;
    static Stopwatch _sw = Stopwatch.StartNew();
    static double _lastWaitForFrameTime = _sw.Elapsed.TotalSeconds;
    static long _emuMemoryOffset = 0;
    static long _emuBackbufferMemoryOffset = 0;
    static readonly object _cacheLockObject = new object();

    static Stopwatch _emuFramerateStopwatch = new Stopwatch();
    static long _emuFramesSinceLastFpsCalculation = 0;
    static double _emuFpsEstimate = 0;

    static bool _emuGameHasStarted = false;

    static byte[] _cacheBuffer = new byte[EMU_MEMORY_SIZE];
    static Dictionary<long, bool> _cacheBlocksReadThisFrame = new Dictionary<long, bool>();

    static Queue<(uint address, byte[] data)> _queuedWrites = new Queue<(uint address, byte[] data)>();

    public static event Action OnPreTick;
    public static event Action OnTick;
    public static event Action OnPostTick;

    public static double EstimatedEmulatorFps => _emuFpsEstimate;
    public static float EmulatorAlpha => Mathf.Clamp01((float)((_sw.Elapsed.TotalMilliseconds - _lastFrameMs) / 16.666666));
    public static bool ConnectedAndGameStarted => _pine.Value != null && _pine.Value.Connected && _emuGameHasStarted;
    public static bool Connected => _pine.Value != null && _pine.Value.Connected;
    public static bool Connecting => _pine.Value != null && _pine.Value.Connecting && !_emuGameHasStarted;
    public static bool InTick { get; private set; } = false;

    static IntPtr GetProcessMemoryOffset(long eeMemoryAddress)
    {
        return (IntPtr)(_emuMemoryOffset + eeMemoryAddress);
    }

    public static bool StartNewPine(bool waitForConnection)
    {
        try
        {
            if (_pine.Value != null)
            {
                _pine?.Value?.Close();
                _pine.Value = null;
            }

            _pine.Value = new PINE(_pinePort);

            while (waitForConnection && _pine.Value.Connecting)
                Thread.Sleep(1);

            if (_bgThread != null)
            {
                _bgThread.Abort();
                _bgThread = null;
            }

            _bgThread = new Thread(ThreadStart);
            _bgThread.IsBackground = true;
            _bgThread.Start();

            return _pine.Value.Connected;
        }
        catch (Exception e)
        {
            //UnityEngine.Debug.LogError(e);
            return false;
        }
    }

    static void ThreadStart()
    {
        int lGt = 0;
        double lGtMs = 0;
        var timeout = TimeSpan.FromMilliseconds(1);
        var commandBufferResetBytes = new byte[5];

        try
        {
            _pine.Value = new PINE(_pinePort + 1);
            while (_pine.Value.Connecting)
                Thread.Sleep(1);

            // reset fps estimate counters
            _emuFramerateStopwatch.Start();
            _emuFpsEstimate = 0;
            _emuFramesSinceLastFpsCalculation = 0;

            while (_bgThread != null && _pine.Value != null && _pine.Value.Connected)
            {
                var pine = GetPINE();

                if (pine != null && _emuMemoryOffset == 0)
                {
                    if (pine.GetVmPtr().TryGetResult(out var vmPtrOffsets))
                    {
                        _emuMemoryOffset = (long)vmPtrOffsets.RealtimeMemoryOffset;
                        _emuBackbufferMemoryOffset = (long)vmPtrOffsets.BackbufferMemoryOffset;
                        UnityEngine.Debug.Log($"{_emuMemoryOffset:X8} {_emuBackbufferMemoryOffset:X8}");
                    }
                }

#if UNITY_EDITOR
                var cacheUpdateStart = _sw.Elapsed.TotalMilliseconds;
#endif

                // poll
                if (_emuMemoryOffset != 0 && pine != null && pine.GetStatus().TryGetResult(out var status))
                {
                    if (!_emuGameHasStarted && status.Code == IPCStatusCode.Running)
                        _emuGameHasStarted = true;

                    // indicate we've hit a new frame
                    if (!_tickQueued && status.Frame != _lastFrame)
                    {
                        DebugStats.EMUTimeMs = _sw.Elapsed.TotalMilliseconds - _lastFrameMs;
                        var ms = _lastFrameMs = _sw.Elapsed.TotalMilliseconds;
                        _cacheBlocksReadThisFrame.Clear();
                        _lastFrame = status.Frame;
                        _emuFramesSinceLastFpsCalculation++;

                        if (_emuFramerateStopwatch.Elapsed.TotalSeconds > 1)
                        {
                            _emuFpsEstimate = _emuFramesSinceLastFpsCalculation / _emuFramerateStopwatch.Elapsed.TotalSeconds;
                            _emuFramerateStopwatch.Restart();
                            _emuFramesSinceLastFpsCalculation = 0;
                        }

                        // acquire lock on cache
                        //InvalidateCache();
                        //lock (_cacheLockObject)
                        {
                            // remove old caches
                            var keys = _memoryCache.Keys.ToList();
                            foreach (var key in keys)
                            {
                                var cache = _memoryCache[key];
                                var blockId = key / CACHE_BLOCK_SIZE;
                                var blockOffset = blockId * CACHE_BLOCK_SIZE;
                                if ((_lastFrame - cache.FrameLastRequested) > 5)
                                {
                                    _memoryCache.TryRemove(key, out _);
                                }
                                else
                                {
                                    cache.Valid = true;
                                    cache.FrameLastUpdated = _lastFrame;

                                    if (!ReadCacheBlocks(cache.Address, cache.Length))
                                        cache.Valid = false;
                                }
                            }
                        }

                        // read command
                        if (ReadCacheBlocks(Constants.CmdInAddress - 4, 4))
                        {
                            var cmdBufInSize = BitConverter.ToInt32(_cacheBuffer, (int)Constants.CmdInAddress - 4);
                            if (cmdBufInSize > 0)
                                ReadCacheBlocks(Constants.CmdInAddress, cmdBufInSize + 1);

                            _queuedWrites.Enqueue((Constants.CmdInAddress - 4, commandBufferResetBytes));
                        }

                        // write
                        lock (_cacheLockObject)
                        {
                            while (_queuedWrites.TryDequeue(out var write))
                            {
                                pine.WriteBytes(write.address, write.data, 0, write.data.Length);
                            }
                        }

                        // 
                        DebugStats.EMUCacheReadTimeMs = _sw.Elapsed.TotalMilliseconds - ms;

                        //_frameHasFinished = true;
                        _tickQueued = true;
                    }
                }

#if UNITY_EDITOR
                if (_tickQueued)
                {
                    var blockCount = _cacheBlocksReadThisFrame.Count;
                    var cacheUpdateDt = _sw.Elapsed.TotalMilliseconds - cacheUpdateStart;
                    //UnityEngine.Debug.Log($"CACHE UPDATE {cacheUpdateDt:0.00} ms ({blockCount} blocks of 0x{CACHE_BLOCK_SIZE:X})");
                }
#endif


                Thread.Sleep(timeout);
            }
        }
        catch (ThreadAbortException) { }
        catch (Exception ex)
        {
#if !UNITY_EDITOR
            Dispatcher.RunOnMainThread(() =>
            {
                UnityEngine.Debug.LogException(ex);
            });
#endif
        }
        finally
        {
            _pine.Value?.Close();
            _pine.Value = null;
        }
    }

    static bool CacheHasBlock(uint blockId)
    {
        return _cacheBlocksReadThisFrame.TryGetValue(blockId, out var readThisFrame) && readThisFrame;
    }

    static bool CacheHasBlocks(uint address, int length)
    {
        var blockIdStart = address / CACHE_BLOCK_SIZE;
        var blockIdEnd = (address + length) / CACHE_BLOCK_SIZE;

        // an address and length can span multiple blocks
        // check if we've got all of them or return false
        for (var blockId = blockIdStart; blockId <= blockIdEnd; ++blockId)
        {
            if (!CacheHasBlock(blockId))
                return false;
        }

        return true;
    }

    static bool ReadCacheBlocks(uint address, int length)
    {
        var blockIdStart = address / CACHE_BLOCK_SIZE;
        var blockIdEnd = (address + length) / CACHE_BLOCK_SIZE;

        // an address and length can span multiple blocks
        // try and read each block in span
        for (var blockId = blockIdStart; blockId <= blockIdEnd; ++blockId)
        {
            // skip blocks in span we've already cached
            if (CacheHasBlock(blockId))
                continue;

            var blockOffset = blockId * CACHE_BLOCK_SIZE;
            if (!_emuMem.ReadByteArray(GetProcessMemoryOffset(blockOffset), _cacheBuffer, blockOffset, CACHE_BLOCK_SIZE))
                return false;

            _cacheBlocksReadThisFrame[blockId] = true;
        }

        return true;
    }

    public static bool? SpawnEmu()
    {
        //return ConnectToEmuProcess();

        if (_emuProcessId != null)
        {
            ProcessHelper.KillProcess(_emuProcessId.Value);
            _emuProcessId = null;
        }

        // reset port to default
        _pinePort = 28011;

        // reset cached memory offset
        _emuMemoryOffset = 0;

        // generate random port for pine
        var newPort = UnityEngine.Random.Range(28100, 28400);

        var emuPath = Constants.GetEmuPath();
        var flags = $"-pineslot {newPort}";


#if !UNITY_EDITOR
        var args = Environment.GetCommandLineArgs();
        if (args == null || !args.Contains("showgui"))
            flags += $" -nogui";
#endif

        // determine if AVX2 or regular build
        var buildType = "";
        if (Config.Singleton.EmulatorBuildType == 1 || (Config.Singleton.EmulatorBuildType == 0 && ProcessorHelper.ProcessorSupportsAVX2()))
            buildType = "-avx2";

        // spawn emulator with Realtime process priority
        if (ProcessHelper.Start(Path.Combine(emuPath, $"pcsx2-qtx64{buildType}.exe"), $"\"{Config.Singleton.IsoPath}\" {flags}", emuPath, false, out var pid))
        {
            _emuProcessId = pid;
            _emuMem = new ProcessMemory(pid, true);
            _pinePort = newPort;
            ProcessHelper.SetProcessPriority(pid, ProcessPriorityClass.RealTime);
        }
        else
        {
            return null;
        }

        // set our process priority to realtime
        ProcessHelper.SetProcessPriority((uint)ProcessHelper.GetCurrentProcessID(), ProcessPriorityClass.AboveNormal);


        return false;
    }

    public static bool? ConnectToEmuProcess()
    {
        // reset port to default
        _pinePort = 28011;

        // reset cached memory offset
        _emuMemoryOffset = 0;

        var process = Process.GetProcessesByName("pcsx2-qtx64-avx2-dbg").FirstOrDefault();
        if (process != null)
        {
            _emuProcessId = (uint)process.Id;
            _emuMem = new ProcessMemory((uint)process.Id, true);
            ProcessHelper.SetProcessPriority((uint)process.Id, ProcessPriorityClass.RealTime);
        }

        // set our process priority to realtime
        ProcessHelper.SetProcessPriority((uint)ProcessHelper.GetCurrentProcessID(), ProcessPriorityClass.AboveNormal);

        return false;
    }

    public static void KillEmu()
    {
        if (_emuProcessId.HasValue)
        {
            ProcessHelper.KillProcess(_emuProcessId.Value);
            _emuProcessId = null;
        }

        _emuMem = null;
    }

    static PINE GetPINE()
    {
        return _pine.Value;
    }

    public static void Dispose()
    {
        _emuGameHasStarted = false;
        _memoryCache?.Clear();

        _bgThread?.Abort();
        _bgThread = null;

        _pine.Value?.Close();
        _pine.Value = null;
    }

#region Misc
    
    public static void Tick()
    {
        // wait for frame
        // if wait for frame fails
        // exit without triggering tick
        if (!WaitForFrame())
        {
            DebugStats.EMUWaitForFrameSkips++;
            return;
        }

        try
        {
            var ms = _sw.Elapsed.TotalMilliseconds;

            // tick
            InTick = true;
            OnPreTick?.Invoke();
            OnTick?.Invoke();
            OnPostTick?.Invoke();

            DebugStats.EMUInteropTickTimeMs = _sw.Elapsed.TotalMilliseconds - ms;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }
        finally
        {
            InTick = false;
            InvalidateCache();
            //FlushCommandBuffer();
            FrameAdvance();
            _tickQueued = false;
        }
    }

    public static bool WaitForFrame()
    {
        var startTimeMs = _sw.ElapsedMilliseconds;

        // sometimes unity will run multiple physics updates sequentially to try and catch up on lost updates (due to lag or OS throttling)
        // we don't want to wait on the emu to finish another tick if we're catching up
        if ((_sw.Elapsed.TotalSeconds - _lastWaitForFrameTime) < Time.fixedDeltaTime)
            return _tickQueued;

        // wait for frame to finish processing
        var pine = GetPINE();
        while (!_tickQueued && pine != null && pine.Connected)
        {
            Thread.Sleep(1);

            // skip this frame if we've waited a full tick (~16.6ms)
            // this is a safety precaution to prevent the game locking up
            // when one tick takes longer than it should, causing the next FixedUpdate
            // to loop until the game catches up
            // which won't happen until the tick time returns to sub 16ms
            if ((_sw.ElapsedMilliseconds - startTimeMs) >= 16)
            {
                //UnityEngine.Debug.Log($"waitforframe safe timedout {Time.time}");
                //SnackManager.AddSnack($"waitforframe safe timedout {Time.time}");
                _lastWaitForFrameTime = _sw.Elapsed.TotalSeconds;
                return false;
            }
        }

        _lastWaitForFrameTime = _sw.Elapsed.TotalSeconds;
        return true;
    }

    public static void Resume()
    {
        var pine = GetPINE();
        if (pine == null)
            return;

        pine.Resume().TryGetResult(out _);
    }

    public static void Pause()
    {
        var pine = GetPINE();
        if (pine == null)
            return;

        pine.Pause().TryGetResult(out _);
    }

    public static void Restart()
    {
        var pine = GetPINE();
        if (pine == null)
            return;

        pine.Restart().TryGetResult(out _);
    }

    public static void Stop()
    {
        var pine = GetPINE();
        if (pine == null)
            return;

        pine.Stop().TryGetResult(out _);
    }

    public static void LoadSaveState(int stateIdx)
    {
        var pine = GetPINE();
        if (pine == null)
            return;

        pine.LoadSaveState(stateIdx).TryGetResult(out _);
    }

    public static void FrameAdvance()
    {
        var pine = GetPINE();
        if (pine == null)
            return;
        
        pine.FrameAdvance().TryGetResult(out _);
    }

    public static uint? GetFrameCount()
    {
        var pine = GetPINE();
        if (pine == null)
            return null;

        if (pine.GetStatus().TryGetResult(out var status))
            return status.Frame;

        return null;
    }

    public static PINE.Promise<IPCStatus> GetStatus()
    {
        var pine = GetPINE();
        if (pine == null)
            return PINE.Promise<IPCStatus>.Empty;

        return pine.GetStatus();
    }

    public static void FrameAdvanceAndWait()
    {
        var pine = GetPINE();
        if (pine == null)
            return;

        // get status
        if (!pine.GetStatus().TryGetResult(out var firstStatus))
            return;

        if (pine.FrameAdvance().TryGetResult(out _))
        {
            while (true)
            {
                // get status
                if (!pine.GetStatus().TryGetResult(out var status))
                    break;

                if (status.Frame != firstStatus.Frame)
                    break;
            }
        }
    }

    public static PINE.Promise<bool> GetFrameBuffer(byte[] buffer)
    {
        var pine = GetPINE();
        if (pine == null)
            return PINE.Promise<bool>.Empty;

        return pine.GetFrameBuffer(buffer);
    }

    public static PINE.Promise<bool> SetPad(int port, int slot, byte[] padData)
    {
        var pine = GetPINE();
        if (pine == null)
            return PINE.Promise<bool>.Empty;

        return pine.SetPad(port, slot, padData);
    }

    public static PINE.Promise<bool> SetFrameSleepWait(bool sleepWhileWaiting)
    {
        var pine = GetPINE();
        if (pine == null)
            return PINE.Promise<bool>.Empty;

        return pine.SetFrameSleepWait(sleepWhileWaiting);
    }

    public static PINE.Promise<bool> SetDisableRendering(bool disableRendering)
    {
        var pine = GetPINE();
        if (pine == null)
            return PINE.Promise<bool>.Empty;

        return pine.SetDisableRendering(disableRendering);
    }

#endregion

    #region Write

    public static bool WriteBytes(uint address, byte[] data)
    {
        var pine = GetPINE();
        if (pine == null)
            return false;

        return pine.WriteBytes(address, data, 0, data.Length);
    }

    public static bool WriteFloat(uint address, float value)
    {
        var pine = GetPINE();
        if (pine == null)
            return false;

        return pine.WriteBytes(address, BitConverter.GetBytes(value), 0, 4);
    }

    public static bool WriteInt64(uint address, long value)
    {
        var pine = GetPINE();
        if (pine == null)
            return false;

        return pine.WriteBytes(address, BitConverter.GetBytes(value), 0, 8);
    }

    public static bool WriteInt32(uint address, int value)
    {
        var pine = GetPINE();
        if (pine == null)
            return false;

        return pine.WriteBytes(address, BitConverter.GetBytes(value), 0, 4);
    }

    public static bool WriteInt16(uint address, short value)
    {
        var pine = GetPINE();
        if (pine == null)
            return false;

        return pine.WriteBytes(address, BitConverter.GetBytes(value), 0, 2);
    }

    public static bool WriteInt8(uint address, byte value)
    {
        var pine = GetPINE();
        if (pine == null)
            return false;

        return pine.WriteBytes(address, new byte[] { value }, 0, 1);
    }

    #endregion

    #region Read

    public static void InvalidateCache()
    {
        // reset
        //lock (_cacheLockObject)
        {
            foreach (var kvp in _memoryCache)
                kvp.Value.Valid = false;
        }
    }

    public static void FlushCommandBuffer()
    {
        var pine = GetPINE();
        if (pine != null)
            pine.SendCommandsAndWait();
    }

    public static ByteSlice? ReadBytes(uint address, int length, bool ignoreCache = false)
    {
        if (length <= 0)
            return null;

        try
        {
            // check cache
            if (!ignoreCache)
            {
                if (!InTick)
                {
                    UnityEngine.Debug.LogException(new Exception("Not InTick cache read"));
                }

                //lock (_cacheLockObject)
                {
                    if (_memoryCache.TryGetValue(address, out var cache) && cache.Valid && cache.Length >= length)
                    {
                        cache.FrameLastRequested = _lastFrame;
                        if (cache.Length == length)
                        {
                            return cache.CachedValue;
                        }
                        else
                        {
                            return new ByteSlice(_cacheBuffer, address, (uint)length);
                        }
                    }
                }
            }

            if (_emuMem != null)
            {
                if (!ReadCacheBlocks(address, length))
                    return null;

                if (!ignoreCache)
                {
                    //lock (_cacheLockObject)
                    {
                        _memoryCache.AddOrUpdate(address, (_) => new MemoryCache()
                        {
                            Address = address,
                            CachedValue = new ByteSlice(_cacheBuffer, address, (uint)length),
                            Valid = true,
                            FrameLastRequested = _lastFrame,
                            Length = (int)length
                        }, (_, cache) => { cache.Length = Math.Max(cache.Length, (int)length); cache.FrameLastRequested = _lastFrame; return cache; });
                    }
                }

                return new ByteSlice(_cacheBuffer, address, (uint)length);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        return null;
    }

    public static string ReadString(uint address, int length)
    {
        if (length <= 0)
            return null;

        var data = ReadBytes(address, length);
        if (data == null)
            return null;

        return data.Value.ToString(0);
    }

    public static Vector3? ReadVector3(uint address)
    {
        var data = ReadBytes(address, 12);
        if (data == null)
            return null;

        var x = data.Value.ToSingle(0);
        var y = data.Value.ToSingle(4);
        var z = data.Value.ToSingle(8);
        return new Vector3(x, y, z);
    }

    public static Matrix3x3? ReadMatrix3x3(uint address)
    {
        var data = ReadBytes(address, 48);
        if (data == null) return null;

        var m = new Matrix3x3()
        {
            row0 = new Vector4(data.Value.ToSingle(0)
                            , data.Value.ToSingle(4)
                            , data.Value.ToSingle(8)
                            , data.Value.ToSingle(12)
                            ),
            row1 = new Vector4(data.Value.ToSingle(16)
                            , data.Value.ToSingle(20)
                            , data.Value.ToSingle(24)
                            , data.Value.ToSingle(28)
                            ),
            row2 = new Vector4(data.Value.ToSingle(32)
                            , data.Value.ToSingle(36)
                            , data.Value.ToSingle(40)
                            , data.Value.ToSingle(44)
                            ),
        };
        return m;
    }

    public static float? ReadFloat(uint address)
    {
        var data = ReadBytes(address, 4);
        return data?.ToSingle(0);
    }

    public static int? ReadInt8(uint address)
    {
        var data = ReadBytes(address, 1);
        return data?[0];
    }

    public static int? ReadInt16(uint address)
    {
        var data = ReadBytes(address, 2);
        return data?.ToInt16(0);
    }

    public static int? ReadInt32(uint address)
    {
        var data = ReadBytes(address, 4);
        return data?.ToInt32(0);
    }

    public static ulong? ReadUInt64(uint address)
    {
        var data = ReadBytes(address, 8);
        return data?.ToUInt64(0);
    }

#endregion

}

class MemoryCache
{
    private bool _valid = false;

    public uint Address { get; set; }
    public ByteSlice CachedValue { get; set; }
    public int Length { get; set; }
    public bool Valid
    {
        get => _valid && CachedValue.Length == Length;
        set => _valid = value;
    }
    public uint FrameLastUpdated { get; set; }
    public uint FrameLastRequested { get; set; }
}


public class ProcessMemory
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint dwSize, ref uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private unsafe static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte* lpBuffer, uint dwSize, ref uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, ref uint lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool GetExitCodeProcess(IntPtr hObject, out uint lpExitCode);

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr hObject); // unused?

    [DllImport("kernel32.dll")]
    private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect); // unused?

    [DllImport("kernel32.dll")]
    private static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

    [DllImport("kernel32.dll")]
    private static extern uint SuspendThread(IntPtr hThread);

    [DllImport("kernel32.dll")]
    private static extern int ResumeThread(IntPtr hThread);

    public bool TrustProcess;
    private bool _opened;
    private uint mainProcessId;
    private IntPtr processHandle = IntPtr.Zero;

    public ProcessMemory(uint pId, bool trust = false)
    {
        mainProcessId = pId;
        TrustProcess = trust;
    }

    public bool CheckProcess()
    {
        if (TrustProcess && _opened) return true;
        if (mainProcessId == 0) return false;

        if (GetExitCodeProcess(processHandle, out uint code) && code != 259)
        {
            CloseHandle(processHandle);
            processHandle = IntPtr.Zero;
        }

        if (processHandle == IntPtr.Zero)
        {
            processHandle = OpenProcess(0x001F0FFF, false, mainProcessId);
            if (processHandle == IntPtr.Zero) return false;
        }

        if (TrustProcess) _opened = true;
        return true;
    }

    public bool ReadByteArray(IntPtr addr, uint size, out byte[] output)
    {
        output = null;
        try
        {
            if (!CheckProcess()) return false;

            VirtualProtectEx(processHandle, addr, (UIntPtr)size, 0x40 /* rw */, out uint flNewProtect);

            output = new byte[size];
            uint bytesRead = 0;
            var read = ReadProcessMemory(processHandle, addr, output, size, ref bytesRead);
            if (read && bytesRead != size)
            {
                UnityEngine.Debug.LogError($"READ FAILED AT {addr:X16} for {size} bytes. Read {bytesRead} instead.");
                read = false;
            }

            VirtualProtectEx(processHandle, addr, (UIntPtr)size, flNewProtect, out _);
            return read;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        return false;
    }

    public unsafe bool ReadByteArray(IntPtr addr, byte[] dest, uint offset, uint size)
    {
        bool read = false;
        try
        {
            if (!CheckProcess()) return false;
            if (dest == null || (offset + size) > dest.Length) throw new InvalidOperationException("Attempting to read memory past the bounds of the destination buffer.");

            //VirtualProtectEx(processHandle, addr, (UIntPtr)size, 0x40 /* rw */, out uint flNewProtect);

            fixed (byte* pDest = dest)
            {
                uint bytesRead = 0;
                read = ReadProcessMemory(processHandle, addr, pDest + offset, (uint)size, ref bytesRead);
                if (read && bytesRead != size)
                {
                    UnityEngine.Debug.LogError($"READ FAILED AT {addr:X16} for {size} bytes. Read {bytesRead} instead.");
                    read = false;
                }
            }

            //VirtualProtectEx(processHandle, addr, (UIntPtr)size, flNewProtect, out _);
            return read;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        return false;
    }

    public bool WriteByteArray(IntPtr addr, byte[] pBytes)
    {
        try
        {
            if (!CheckProcess()) return false;

            VirtualProtectEx(processHandle, addr, (UIntPtr)pBytes.Length, 0x40 /* rw */, out uint flNewProtect);

            uint bytesWritten = 0;
            bool flag = WriteProcessMemory(processHandle, addr, pBytes, (uint)pBytes.Length, ref bytesWritten);

            VirtualProtectEx(processHandle, addr, (UIntPtr)pBytes.Length, flNewProtect, out _);

            return flag;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        return false;
    }
}
