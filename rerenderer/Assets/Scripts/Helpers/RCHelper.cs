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
    public static readonly Dictionary<uint, char> DEADLOCKED_EXTENDED_ASCII_REMAP = new Dictionary<uint, char>()
    {
        { 0x02, '¡' },
        { 0x03, '©' },
        // { 0x04, 'idk' },
        { 0x05, '«' },
        { 0x06, '®' },
        { 0x07, '¢' },
        // { 0x08, 'idk' },
        { 0x09, '»' },
        { 0x0a, '¿' },
        { 0x0b, 'À' },
        { 0x0c, 'Á' },
        { 0x0d, 'Â' },
        { 0x0e, 'Ã' },
        { 0x0f, 'Ä' },
        { 0x10, 'Å' },
        { 0x11, 'Æ' },
        { 0x12, 'Ç' },
        { 0x13, 'È' },
        { 0x14, 'É' },
        { 0x15, 'Ê' },
        { 0x16, 'Ë' },
        { 0x17, 'Ì' },
        { 0x18, 'Í' },
        { 0x19, 'Î' },
        { 0x1a, 'Ï' },
        { 0x1b, 'Ñ' },
        { 0x1c, 'Ò' },
        { 0x1d, 'Ó' },
        { 0x1e, 'Ô' },
        { 0x1f, 'Õ' },
        { 0x20, 'Ö' },
        { 0x21, 'Ø' },
        { 0x22, 'Ù' },
        { 0x23, 'Ú' },
        { 0x24, 'Û' },
        // { 0x25, 'does not exist' },
        { 0x26, 'Ü' },
        { 0x27, 'ß' },
        { 0x28, 'à' },
        { 0x29, 'á' },
        { 0x2a, 'â' },
        { 0x2b, 'ã' },
        { 0x2c, 'ä' },
        { 0x2d, 'å' },
        { 0x2e, 'æ' },
        { 0x2f, 'ç' },
        { 0x30, 'è' },
        { 0x31, 'é' },
        { 0x32, 'ê' },
        { 0x33, 'ë' },
        { 0x34, 'ì' },
        { 0x35, 'í' },
        { 0x36, 'î' },
        { 0x37, 'ï' },
        { 0x38, 'ñ' },
        { 0x39, 'ò' },
        { 0x3a, 'ó' },
        { 0x3b, 'ô' },
        { 0x3c, 'õ' },
        { 0x3d, 'ö' },
        { 0x3e, 'ø' },
        { 0x3f, 'ù' },
        { 0x40, 'ú' },
        { 0x41, 'û' },
        { 0x42, 'ü' },
        { 0x43, 'ÿ' },
        { 0x44, 'Œ' },
        { 0x45, 'œ' },
        { 0x46, 'Š' },
        { 0x47, 'š' },
        { 0x48, 'Ÿ' },
        // { 0x49, 'idk' },
        // { 0x4a, 'idk' },
        { 0x4b, '“' },
        { 0x4c, '”' },
        { 0x4d, '„' },
        { 0x4e, '…' },
        { 0x4f, '€' },
        // { 0x50, 'black line' },
    };

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

            if ((c & 0x80) != 0)
            {
                // remap extended to our extended
                if (DEADLOCKED_EXTENDED_ASCII_REMAP.TryGetValue(rcString[i + 1], out var extChar))
                    sb.Append(extChar);
                else
                    sb.Append('■'); // default

                i += 1; // skip extended code
            }
            else
            {

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
