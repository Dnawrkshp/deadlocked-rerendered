using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public struct ByteSlice : IEnumerable<byte>
{
    byte[] buffer;
    uint offset;
    uint length;


    public byte[] Buffer => buffer;
    public uint Offset => offset;
    public uint Length => length;

    public bool HasSlice => buffer != null && offset >= 0 && (offset + length) < buffer.Length;
    public byte this[int idx] => buffer[offset + idx];
    public byte this[uint idx] => buffer[offset + idx];

    public ByteSlice(byte[] buffer, uint offset, uint length)
    {
        this.buffer = buffer;
        this.offset = offset;
        this.length = length;
    }

    public unsafe string ToString(int index = 0, int? strLen = null)
    {
        if (!HasSlice)
            return string.Empty;

        return Encoding.UTF8.GetString(buffer, (int)(offset + index), strLen ?? (int)length);
    }

    public unsafe sbyte ToSByte(int index = 0)
    {
        sbyte result = 0;
        if (!HasSlice || length < sizeof(sbyte))
            return result;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            result = *(sbyte*)pBuffer;
        }

        return result;
    }

    public unsafe short ToInt16(int index = 0)
    {
        short result = 0;
        if (!HasSlice || length < sizeof(short))
            return result;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            result = *(short*)pBuffer;
        }

        return result;
    }

    public unsafe uint ToUInt32(int index = 0)
    {
        uint result = 0;
        if (!HasSlice || length < sizeof(uint))
            return result;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            result = *(uint*)pBuffer;
        }

        return result;
    }

    public unsafe int ToInt32(int index = 0)
    {
        int result = 0;
        if (!HasSlice || length < sizeof(int))
            return result;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            result = *(int*)pBuffer;
        }

        return result;
    }

    public unsafe float ToSingle(int index = 0)
    {
        float result = 0;
        if (!HasSlice || length < sizeof(float))
            return result;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            result = *(float*)pBuffer;
        }

        return result;
    }

    public unsafe ulong ToUInt64(int index = 0)
    {
        ulong result = 0;
        if (!HasSlice || length < sizeof(ulong))
            return result;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            result = *(ulong*)pBuffer;
        }

        return result;
    }

    public unsafe byte* Ref(long index = 0)
    {
        if (!HasSlice)
            return null;

        fixed (byte* pBuffer = &buffer[offset + index])
        {
            return pBuffer;
        }
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < length; ++i)
        {
            yield return buffer[i + offset];
        }
    }

    IEnumerator<byte> IEnumerable<byte>.GetEnumerator()
    {
        for (int i = 0; i < length; ++i)
        {
            yield return buffer[i + offset];
        }
    }
}

public class PadData
{
    public bool IsGameKBM { get; set; }
    public bool IsNavKBM { get; set; }
    public Vector2 LookDelta { get; set; }
    public PadButtons Buttons { get; set; }
    public float RightHorizontal { get; set; }
    public float RightVertical { get; set; }
    public float LeftHorizontal { get; set; }
    public float LeftVertical { get; set; }
    public float LeftTrigger { get; set; }
    public float RightTrigger { get; set; }

    private static byte RawJoystickToByte(float value)
    {
        value = Mathf.Clamp01((1 + value) / 2);
        return (byte)(value * 255);
    }

    public void WriteBytes(byte[] buffer)
    {
        var buttonMask = (ushort)(0xFFFF & ~(int)Buttons);

        buffer[0] = 0; // ok
        buffer[1] = 0x79; // mode
        buffer[2] = (byte)buttonMask;
        buffer[3] = (byte)(buttonMask >> 8);
        buffer[4] = RawJoystickToByte(RightHorizontal);
        buffer[5] = RawJoystickToByte(RightVertical);
        buffer[6] = RawJoystickToByte(LeftHorizontal);
        buffer[7] = RawJoystickToByte(LeftVertical);
        buffer[8] = (byte)(Buttons.HasFlag(PadButtons.Right) ? 0xFF : 0x00);
        buffer[9] = (byte)(Buttons.HasFlag(PadButtons.Left) ? 0xFF : 0x00);
        buffer[10] = (byte)(Buttons.HasFlag(PadButtons.Up) ? 0xFF : 0x00);
        buffer[11] = (byte)(Buttons.HasFlag(PadButtons.Down) ? 0xFF : 0x00);
        buffer[12] = (byte)(Buttons.HasFlag(PadButtons.Triangle) ? 0xFF : 0x00);
        buffer[13] = (byte)(Buttons.HasFlag(PadButtons.Circle) ? 0xFF : 0x00);
        buffer[14] = (byte)(Buttons.HasFlag(PadButtons.Cross) ? 0xFF : 0x00);
        buffer[15] = (byte)(Buttons.HasFlag(PadButtons.Square) ? 0xFF : 0x00);
        buffer[16] = (byte)(Buttons.HasFlag(PadButtons.L1) ? 0xFF : 0x00);
        buffer[17] = (byte)(Buttons.HasFlag(PadButtons.R1) ? 0xFF : 0x00);
        buffer[18] = (byte)(Mathf.Clamp01(LeftTrigger) * 255);
        buffer[19] = (byte)(Mathf.Clamp01(RightTrigger) * 255);
        //buffer[20] = (byte)(Buttons.HasFlag(PadButtons.Start) ? 0xFF : 0x00);
        //buffer[21] = (byte)(Buttons.HasFlag(PadButtons.Select) ? 0xFF : 0x00);
        //buffer[22] = (byte)(Buttons.HasFlag(PadButtons.L3) ? 0xFF : 0x00);
        //buffer[23] = (byte)(Buttons.HasFlag(PadButtons.R3) ? 0xFF : 0x00);
    }
}
