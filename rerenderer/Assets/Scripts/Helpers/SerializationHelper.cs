using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SerializationHelper
{

    /// <summary>
    /// Converts the given byte array to the struct
    /// type specified by the template parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static T From<T>(this byte[] bytes, int offset = 0)
    {
        // Pin the managed memory while, copy it out the data, then unpin it
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject() + offset, typeof(T));
        handle.Free();

        return theStructure;
    }

    /// <summary>
    /// Reads in a block from a file and converts it to the struct
    /// type specified by the template parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T FromBinaryReader<T>(this BinaryReader reader)
    {
        try
        {
            // Read in a byte array
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            // Pin the managed memory while, copy it out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return default(T);
        }
    }


    public static string ReadString(this BinaryReader reader, int fixedLength)
    {
        var bytes = reader.ReadBytes(fixedLength);
        var str = System.Text.Encoding.UTF8.GetString(bytes);
        var nullIdx = str.IndexOf('\0');
        if (nullIdx >= 0)
            return str.Substring(0, nullIdx);

        return str;
    }
}
