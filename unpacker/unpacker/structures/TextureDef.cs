using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace unpacker
{

    // Had to use PIF prefix since you cant start with a number
    public enum TextureFormat
    {
        PIF_8BPP_UYA_FORMAT = 0x13,
        PIF_8BPP_DL_FORMAT = 0x93,
        PIF_4BPP_UYA_FORMAT = 0x14,
        PIF_4BPP_DL_FORMAT = 0x94
    };

    public enum FileFormat
    {
        PIF2_DL,
        PIF2_UYA,
        PIF2_ASSET_TEXTURE_DL,
        PIF2_ASSET_TEXTURE_UYA,
        BMP_32BPP_ABGR,
        BMP_32BPP_RGBA,
        BMP_8BPP_INDEXED,
        BMP_4BPP_INDEXED,
        PNG
    }

    public class TextureDef
    {
        public int magic { get; set; } // 2FIP
        public int unk_4 { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public TextureFormat format { get; set; }
        public int unk_14 { get; set; }
        public int unk_18 { get; set; }
        public int unk_1C { get; set; }
        public byte[] palette_data { get; set; }
        public byte[] texture_data { get; set; }

        public TextureDef(BinaryReader reader)
        {
            Read(reader);
        }

        public TextureDef(byte[] textureData, byte[] paletteData, int _width, int _height)
        {
            magic = 0x32464950;
            texture_data = textureData;
            palette_data = paletteData;
            width = _width;
            height = _height;
        }

        public TextureDef(byte[] block)
        {
            using (MemoryStream ms = new MemoryStream(block))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    Read(reader);
                }
            }
        }

        void Read(BinaryReader reader)
        {
            magic = reader.ReadInt32();
            unk_4 = reader.ReadInt32();
            width = reader.ReadInt32();
            height = reader.ReadInt32();
            format = (TextureFormat) reader.ReadInt32();
            unk_14 = reader.ReadInt32();
            unk_18 = reader.ReadInt32();
            unk_1C = reader.ReadInt32();

            switch(format)
            {
                case TextureFormat.PIF_8BPP_UYA_FORMAT:
                default:
                    palette_data = reader.ReadBytes(4 * 256);
                    texture_data = reader.ReadBytes(width * height);
                    break;
                case TextureFormat.PIF_4BPP_DL_FORMAT:
                    palette_data = reader.ReadBytes(4 * 16);
                    texture_data = reader.ReadBytes((width * height)/2);
                    break;
            }
        }

        public void SavePNG(string path, string filename)
        {
            var img = convertPifTo32bppABGRBmp();
            img.Save(Path.Combine(path, filename + Constants.FILE_EXTENSION_PNG));
        }

        Image convertPifTo32bppABGRBmp()
        {
            bool shouldRemapPixel = false;
            using (MemoryStream ms = new MemoryStream(palette_data))
            {
                using (BinaryReader paletteReader = new BinaryReader(ms))
                {
                    int pixelCount = width * height;
                    byte[] imageData = new byte[pixelCount * 4];
                    int startPos = 0;
                    for (int i = 0; i < pixelCount; ++i)
                    {
                        int map = shouldRemapPixel ? Helpers.RemapPixelIndexFromRac4(i, width) : i;
                        int paletteIndex = shouldRemapPaletteIndex() ? Helpers.DecodePaletteIndex(texture_data[i]) : texture_data[i];
                        paletteReader.BaseStream.Position = paletteIndex * 4 + startPos;
                        byte[] rgba = paletteReader.ReadBytes(4);
                        imageData[(map * 4) + 3] = rgba[0];
                        imageData[(map * 4) + 2] = rgba[1];
                        imageData[(map * 4) + 1] = rgba[2];
                        imageData[(map * 4) + 0] = rgba[3];
                    }

                    return Image.LoadPixelData<Abgr32>(imageData, width, height);
                }
            }
        }

        byte[] remapPixels8bpp(byte[] input)
        {
            byte[] output = new byte[input.Length];
            int remappedIndex = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width) + x;
                    int mappedIndex1 = Helpers.RemapPixelIndexFromRac4(pixelIndex, width);
                    byte data = input[mappedIndex1];
                    output[remappedIndex] = (byte)data;
                    remappedIndex++;
                }
            }

            return output;
        }

        byte[] remapPixels4bpp(byte[] input)
        {
            byte[] output = new byte[texture_data.Length * 2];

            int expandedIndex = 0;
            foreach (byte pair in texture_data)
            {
                byte left = (byte)(pair >> 4);
                byte right = (byte)(pair & 0xf);
                output[expandedIndex] = right;
                output[expandedIndex + 1] = left;
                expandedIndex += 2;
            }
            output = remapPixels8bpp(output);

            byte[] compressedData = new byte[(width * height) / 2];
            int compressedIndex = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x += 2)
                {
                    int index1 = (y * width) + x;
                    int index2 = (y * width) + (x + 1);
                    byte left = output[index1];
                    byte right = output[index2];
                    compressedData[compressedIndex] = (byte)((right << 4) | left);
                    compressedIndex++;
                }
            }
            output = compressedData;

            return output;

        }

        bool shouldRemapPixelIndex()
        {
            switch(format)
            {
                case TextureFormat.PIF_4BPP_DL_FORMAT:
                    return true;
                case TextureFormat.PIF_4BPP_UYA_FORMAT:
                    return false;
                default:
                    return true;
            }
        }

        bool shouldRemapPaletteIndex()
        {
            switch (format)
            {
                case TextureFormat.PIF_4BPP_DL_FORMAT:
                case TextureFormat.PIF_4BPP_UYA_FORMAT:
                    return false;
                default:
                    return true;
            }
        }
    }
}
