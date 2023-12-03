using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unpacker
{
    public static class UnpackLevelSprites
    {
        class Sprite
        {
            public int SpriteIndex;
            public ushort? SpritePrefix;
            public ushort? SpriteId;
            public int Width;
            public int Height;
            public int TextureId;
            public int PaletteId;
            public bool IsSprite1;
        }

        public static void Unpack(BinaryReader isoReader, LevelWadDef wadDef, string outDir)
        {
            var palettes = new List<byte[]>();
            var textures = new List<byte[]>();
            var sprites = new List<Sprite>();

            var spriteDir = Path.Combine(outDir, "fx");
            if (!Directory.Exists(spriteDir)) Directory.CreateDirectory(spriteDir);

            isoReader.BaseStream.Seek(wadDef.Offset + Constants.DL.LEVEL_SPRITE_HEADER_OFFSET, SeekOrigin.Begin);
            var spriteHeaderOffset = wadDef.Offset + isoReader.ReadInt32();
            var spriteHeaderSize = isoReader.ReadInt32();
            var spriteHeaderData = isoReader.ReadBytes(spriteHeaderOffset, spriteHeaderSize);

            isoReader.BaseStream.Seek(wadDef.Offset + Constants.DL.LEVEL_SPRITE_WAD1_OFFSET, SeekOrigin.Begin);
            var spriteWad1Offset = wadDef.Offset + isoReader.ReadInt32();
            var spriteWad1Size = isoReader.ReadInt32();

            isoReader.BaseStream.Seek(wadDef.Offset + Constants.DL.LEVEL_SPRITE_WAD2_OFFSET, SeekOrigin.Begin);
            var spriteWad2Offset = wadDef.Offset + isoReader.ReadInt32();
            var spriteWad2Size = isoReader.ReadInt32();

            // unpack sprite wads
            var spriteData1 = WADUtil.Decompress(isoReader.ReadBytes(spriteWad1Offset, spriteWad1Size));
            var spriteData2 = WADUtil.Decompress(isoReader.ReadBytes(spriteWad2Offset, spriteWad2Size));

            if (spriteData1 == null || spriteData2 == null)
                throw new Exception("Unable to decompress sprite wads");

            // unpack
            using (MemoryStream headerMs = new MemoryStream(spriteHeaderData, false))
            {
                using (BinaryReader headerReader = new BinaryReader(headerMs))
                {
                    using (MemoryStream sprite1Ms = new MemoryStream(spriteData1, false))
                    {
                        using (BinaryReader spriteWad1Reader = new BinaryReader(sprite1Ms))
                        {
                            using (MemoryStream sprite2Ms = new MemoryStream(spriteData2, false))
                            {
                                using (BinaryReader spriteWad2Reader = new BinaryReader(sprite2Ms))
                                {
                                    SpriteHeader spriteHeader = new SpriteHeader(headerReader);

                                    // Parse palette texture mapping
                                    headerReader.BaseStream.Seek(spriteHeader.PaletteTextureMappingOffset, SeekOrigin.Begin);
                                    for (int i = 0; i < spriteHeader.PaletteTextureMappingCount; i++)
                                    {
                                        short paletteId = headerReader.ReadInt16();
                                        short textureId = headerReader.ReadInt16();

                                        sprites.Add(new Sprite()
                                        {
                                            PaletteId = paletteId,
                                            TextureId = textureId,
                                            SpriteIndex = i, // this index correlates to the index in the sprite table
                                        });
                                    }

                                    // Parse palettes
                                    headerReader.BaseStream.Seek(spriteHeader.PaletteOffsetsListOffset, SeekOrigin.Begin);
                                    for (int i = 0; i < spriteHeader.PaletteCount_1; i++)
                                    {
                                        uint offset = headerReader.ReadUInt32() - (int.MaxValue) - 1;
                                        int padding = headerReader.ReadInt32();

                                        byte[] palette = spriteWad2Reader.ReadBytes(offset, 0x400);
                                        palettes.Add(palette);
                                    }

                                    // Parse textures
                                    headerReader.BaseStream.Seek(spriteHeader.TextureOffsetsListOffset, SeekOrigin.Begin);
                                    int firstWadTextureIndex = 0;
                                    for (int i = 0; i < spriteHeader.TextureCount_Total_1; i++)
                                    {
                                        uint offset = headerReader.ReadUInt32() - (int.MaxValue) - 1;
                                        ushort padding = headerReader.ReadUInt16();
                                        int wPow = headerReader.ReadByte();
                                        int hPow = headerReader.ReadByte();

                                        // Sizes are stored as a number which is an exponent of 2
                                        int width = (int)Math.Pow(2, wPow);
                                        int height = (int)Math.Pow(2, hPow);

                                        int totalSize = width * height;
                                        bool isSprite1 = false;

                                        // Read the first set of tetures from the first wad
                                        if (firstWadTextureIndex < spriteHeader.TextureCount_FirstWad_1)
                                        {
                                            byte[] texture = spriteWad1Reader.ReadBytes(offset, totalSize);
                                            textures.Add(texture);
                                            firstWadTextureIndex++;
                                            isSprite1 = true;
                                        }
                                        else
                                        {
                                            byte[] texture = spriteWad2Reader.ReadBytes(offset, totalSize);
                                            textures.Add(texture);
                                        }

                                        List<Sprite> matchedSprites = sprites.Where(s => s.TextureId == i).ToList();
                                        matchedSprites.ForEach(s =>
                                        {
                                            s.Width = width;
                                            s.Height = height;
                                            s.IsSprite1 = isSprite1;
                                        });
                                    }

                                    // Parse sprite ID and index mapping
                                    headerReader.BaseStream.Seek(spriteHeader.SpriteIDMappingOffset, SeekOrigin.Begin);
                                    for (int i = 0; i < spriteHeader.SpriteIDMappingCount - 1; i++)
                                    {
                                        ushort spriteId = headerReader.ReadUInt16();
                                        ushort unk = headerReader.ReadUInt16();
                                        int textureId = headerReader.ReadInt32();

                                        List<Sprite> matchedSprites = sprites.Where(s => s.SpriteIndex == i).ToList();
                                        matchedSprites.ForEach(s =>
                                        {
                                            s.SpritePrefix = unk;
                                            s.SpriteId = spriteId;
                                        });
                                    }

                                    foreach (Sprite sprite in sprites)
                                    {
                                        byte[] texture = textures[sprite.TextureId];
                                        byte[] palette = palettes[sprite.PaletteId];
                                        TextureDef tex = new TextureDef(texture, palette, sprite.Width, sprite.Height);
                                        tex.SavePNG(spriteDir, $"{sprite.SpriteIndex}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
