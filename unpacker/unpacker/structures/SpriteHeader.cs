using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unpacker
{
    public class SpriteHeader
    {
        /* 0x00 */
        public short SpriteIDMappingCount;
        /* 0x02 */
        public short PaletteTextureMappingCount;
        /* 0x04 */
        public int SpriteIDMappingOffset;
        /* 0x08 */
        public int PaletteTextureMappingOffset;
        /* 0x0C */
        public int PaletteOffsetsListOffset;
        /* 0x10 */
        public int TextureOffsetsListOffset;
        /* 0x14 */
        public int Unk_14;
        /* 0x18 */
        public int PaletteCount_1; // All of these counts are the same that I can tell
        /* 0x1C */
        public int PaletteCount_2;
        /* 0x20 */
        public int PaletteCount_3;
        /* 0x24 */
        public int PaletteCount_4;
        /* 0x28 */
        public int Unk_28;
        /* 0x2C */
        public int Unk_2C;
        /* 0x30 */
        public int Unk_30;
        /* 0x34 */
        public int TextureCount_FirstWad_1;
        /* 0x38 */
        public int TextureCount_Total_1; // All of these are also the same
        /* 0x3C */
        public int TextureCount_Total_2;
        /* 0x40 */
        public int TextureCount_Total_3;
        /* 0x44 */
        public int TextureCount_Total_4;
        /* 0x48 */
        public int Unk_48;
        /* 0x4C */
        public int Unk_4C;
        /* 0x50 */
        public int Unk_50;
        /* 0x54 */
        public int SpriteWad1Size;
        /* 0x58 */
        public int SpriteWad2Size;
        /* 0x5C */
        public int Unk_5C;
        /* 0x60 */
        public byte[]? Padding = null; //0x54 in length



        public SpriteHeader()
        {

        }

        public SpriteHeader(BinaryReader reader)
        {
            Read(reader);
        }

        public void Read(BinaryReader reader)
        {
            SpriteIDMappingCount = reader.ReadInt16();
            PaletteTextureMappingCount = reader.ReadInt16();
            SpriteIDMappingOffset = reader.ReadInt32();
            PaletteTextureMappingOffset = reader.ReadInt32();
            PaletteOffsetsListOffset = reader.ReadInt32();
            TextureOffsetsListOffset = reader.ReadInt32();
            Unk_14 = reader.ReadInt32();
            PaletteCount_1 = reader.ReadInt32();
            PaletteCount_2 = reader.ReadInt32();
            PaletteCount_3 = reader.ReadInt32();
            PaletteCount_4 = reader.ReadInt32();
            Unk_28 = reader.ReadInt32();
            Unk_2C = reader.ReadInt32();
            Unk_30 = reader.ReadInt32();
            TextureCount_FirstWad_1 = reader.ReadInt32();
            TextureCount_Total_1 = reader.ReadInt32();
            TextureCount_Total_2 = reader.ReadInt32();
            TextureCount_Total_3 = reader.ReadInt32();
            TextureCount_Total_4 = reader.ReadInt32();
            Unk_48 = reader.ReadInt32();
            Unk_4C = reader.ReadInt32();
            Unk_50 = reader.ReadInt32();
            SpriteWad1Size = reader.ReadInt32();
            SpriteWad2Size = reader.ReadInt32();
            Unk_5C = reader.ReadInt32();
            Padding = reader.ReadBytes(0x54);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(SpriteIDMappingCount);
            writer.Write(PaletteTextureMappingCount);
            writer.Write(SpriteIDMappingOffset);
            writer.Write(PaletteTextureMappingOffset);
            writer.Write(PaletteOffsetsListOffset);
            writer.Write(TextureOffsetsListOffset);
            writer.Write(Unk_14);
            writer.Write(PaletteCount_1);
            writer.Write(PaletteCount_2);
            writer.Write(PaletteCount_3);
            writer.Write(PaletteCount_4);
            writer.Write(Unk_28);
            writer.Write(Unk_2C);
            writer.Write(Unk_30);
            writer.Write(TextureCount_FirstWad_1);
            writer.Write(TextureCount_Total_1);
            writer.Write(TextureCount_Total_2);
            writer.Write(TextureCount_Total_3);
            writer.Write(TextureCount_Total_4);
            writer.Write(Unk_48);
            writer.Write(Unk_4C);
            writer.Write(Unk_50);
            writer.Write(SpriteWad1Size);
            writer.Write(SpriteWad2Size);
            writer.Write(Unk_5C);
        }
    }
}
