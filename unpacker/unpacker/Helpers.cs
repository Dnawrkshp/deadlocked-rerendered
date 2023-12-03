using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unpacker
{
    public static class Helpers
    {

        public static byte[] ReadBytes(this BinaryReader reader, long offset, int count)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            return reader.ReadBytes(count);
        }

        public static bool IsWADMagic(this BinaryReader reader)
        {
            return WADUtil.VerifyMagic(reader.ReadBytes(3));
        }

        public static int RemapPixelIndexFromRac4(int i, int width)
        {
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


            int x = n + ((m + q * 4) * 4);
            int y = r + (o * 2);

            return (x % width) + (y * width);
        }

        public static byte DecodePaletteIndex(byte index, int high = 4, int low = 3)
        {
            int dif = high - low;
            uint mask1 = (uint)1 << high;
            uint mask2 = (uint)1 << low;
            uint mask3 = ~(mask1 | mask2);
            uint a = (uint)index & mask3;

            return (byte)(((index & mask1) >> dif) | ((index & mask2) << dif) | a);
        }

    }
}
