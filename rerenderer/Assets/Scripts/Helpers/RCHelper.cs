using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RCHelper
{
    public static Vector3 SwizzleXZY(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.z, vector.y);
    }

    public static Vector4 SwizzleXZY(this Vector4 vector)
    {
        return new Vector4(vector.x, vector.z, vector.y, vector.w);
    }

    public static Matrix4x4 SwizzleXZY(this Matrix4x4 matrix)
    {
        return new Matrix4x4(
            new Vector4(matrix.m00, matrix.m20, matrix.m10, matrix.m30),
            new Vector4(matrix.m02, matrix.m22, matrix.m12, matrix.m32),
            new Vector4(matrix.m01, matrix.m21, matrix.m11, matrix.m31),
            new Vector4(matrix.m03, matrix.m23, matrix.m13, matrix.m33)
            );
    }

    public static Matrix4x4 RCMatrixToMatrix(this Matrix4x4 matrix)
    {
        var mXZY = matrix.SwizzleXZY().Safe();

        return Matrix4x4.TRS(mXZY.GetPosition(), matrix.RCMatrixToQuaternion(), mXZY.lossyScale);
    }

    public static Matrix4x4 Safe(this Matrix4x4 matrix)
    {
        float det = matrix.determinant;
        if (Mathf.Abs(det) > 1E-20)
            return matrix;

        return Matrix4x4.TRS(matrix.GetPosition(), Quaternion.identity, Vector3.one * 1E-20f);
    }

    public static Quaternion RCEulerToQuaternion(this Vector3 rcEuler)
    {
        var transformed = new Vector3(rcEuler.x, rcEuler.z - (Mathf.PI / 2f), rcEuler.y) * -Mathf.Rad2Deg;
        return Quaternion.Euler(transformed);
    }

    public static Quaternion RCEulerToQuaternion(this Vector4 rcEuler)
    {
        return ((Vector3)rcEuler).RCEulerToQuaternion();
    }

    public static Quaternion RCMatrixToQuaternion(this Matrix4x4 rcMatrix)
    {
        var rotation = Quaternion.identity;
        var mXZY = rcMatrix.SwizzleXZY();
        if (rcMatrix.ValidTRS())
            rotation = mXZY.rotation;
        else
            rotation = Quaternion.LookRotation(mXZY.MultiplyVector(Vector3.forward), mXZY.MultiplyVector(Vector3.up));

        return rotation * Quaternion.Euler(0, 180, 0);
    }

    public static unsafe Color RCRgbaToColor(this int rgba)
    {
        byte* p = (byte*)&rgba;

        return new Color(p[0] / 255.0f, p[1] / 255.0f, p[2] / 255.0f, p[3] / 128.0f);
    }

    public static unsafe Color RCRgbaToColor(this uint rgba)
    {
        byte* p = (byte*)&rgba;

        return new Color(p[0] / 255.0f, p[1] / 255.0f, p[2] / 255.0f, p[3] / 128.0f);
    }

    public static unsafe void RCLightsToLighting(this ulong lights, out Vector3 direction, out Color color)
    {
        byte* p = (byte*)&lights;

        color = ((uint)(lights >> 32)).RCRgbaToColor();
        direction = new Vector3(0, 1, 1);
    }

    public static bool IsDestroyed(this MobyInstance mobyInstance)
    {
        return mobyInstance.state <= -2;
    }

    public static bool IsDynamicDestroyed(this MobyInstance mobyInstance)
    {
        return mobyInstance.state == -2;
    }

    public static bool IsStaticDestroyed(this MobyInstance mobyInstance)
    {
        return mobyInstance.state == -3;
    }
    
    public static string ConvertRCStringToRichString(this string rcString)
    {
        if (rcString == null)
            return null;

        StringBuilder sb = new StringBuilder();
        int colorCount = 0;

        for (int i = 0; i < rcString.Length; ++i)
        {
            var c = rcString[i];

            switch (c)
            {
                case '\x00': return sb.ToString();

                // default color (close color tags)
                case '\x08':
                    while (colorCount > 0)
                    {
                        sb.Append("</color>");
                        colorCount--;
                    }
                    break;

                // blue
                case '\x09':
                    sb.Append("<color=#8176E2>");
                    colorCount++;
                    break;

                // green
                case '\x0A':
                    sb.Append("<color=#4A9C3D>");
                    colorCount++;
                    break;

                // pink
                case '\x0B':
                    sb.Append("<color=#AD63A5>");
                    colorCount++;
                    break;

                // white
                case '\x0C':
                    sb.Append("<color=#DFD8D8>");
                    colorCount++;
                    break;

                // black
                case '\x0D':
                    sb.Append("<color=#0C0202>");
                    colorCount++;
                    break;

                // red
                case '\x0E':
                    sb.Append("<color=#E11F1F>");
                    colorCount++;
                    break;

                // aqua
                case '\x0F':
                    sb.Append("<color=#4CD1D1>");
                    colorCount++;
                    break;

                // BUTTONS
                case '\x10':
                case '\x11':
                case '\x12':
                case '\x13':
                case '\x14':
                case '\x15':
                case '\x16':
                case '\x17':
                case '\x18':
                case '\x19':
                case '\x1A':
                case '\x1B':
                case '\x1C':
                case '\x1D':
                case '\x1E':
                case '\x1F':
                    sb.Append($"<sprite index={(int)c - 0x10}>");
                    break;

                // empty
                case '\x01': 
                case '\x02': 
                case '\x03': 
                case '\x04': 
                case '\x05': 
                case '\x06':
                case '\x07':
                    break;

                // add to string
                default: sb.Append(c); break;
            }
        }

        return sb.ToString();
    }

    public static bool HasValue(this RCPointer rcPointer)
    {
        return rcPointer.Address != 0;
    }

    public static bool IsRunningDeadlocked()
    {
        // todo: add gladiator mastercode
        var magic = EmuInterop.ReadInt32(0x0013BA48);
        return magic == 0x00832021;
    }

    static (int x, int y) remap_pixel_index_rac4(int x, int y, int width)
    {
        var i = (y * width) + x;

        int s = i / (width * 2);
        int r = 0;
        if (s % 2 == 0)
            r = s * 2;
        else
            r = (s - 1) * 2 + 1;

        int q = ((i % (width * 2)) / 32);

        int m = i % 4;
        int n = (i / 4) % 4;
        int o = i % 2;
        int p = (i / 16) % 2;

        if ((s / 2) % 2 == 1)
            p = 1 - p;

        if (o == 0)
            m = (m + p) % 4;
        else
            m = ((m - p) + 4) % 4;

        // compute new x,y
        x = n + ((m + q * 4) * 4);
        y = r + (o * 2);

        return ((x%width), y);
    }

    static byte decode_palette_index(byte index, int high = 4, int low = 3)
    {
        int dif = high - low;
        uint mask1 = (uint)1 << high;
        uint mask2 = (uint)1 << low;
        uint mask3 = ~(mask1 | mask2);
        uint a = (uint)index & mask3;

        return (byte)(((index & mask1) >> dif) | ((index & mask2) << dif) | a);
    }
}
