using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unpacker
{
    public struct LevelWadDef
    {
        public long Offset;
        public int Size;
    }

    public static class UnpackLevel
    {
        public class InvalidLevelTOCException : Exception
        {
            public InvalidLevelTOCException() { }
            public InvalidLevelTOCException(string message) : base(message) { }
        }

        public static void Unpack(string isoPath, int levelId, string outDir)
        {
            if (!File.Exists(isoPath)) return;

            using (var fs = new FileStream(isoPath, FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    var toc = ReadLevelTocFromIso(br, levelId);
                    if (toc == null) return;

                    Unpack(br, toc, levelId, outDir);
                }
            }

        }

        private static byte[] ReadLevelTocFromIso(BinaryReader isoReader, int levelId)
        {
            var tocOffset = Constants.DL.LEVEL_WAD_TOC + 0x18 * levelId;

            isoReader.BaseStream.Seek(tocOffset, SeekOrigin.Begin);
            var levelTocStart = isoReader.ReadUInt32() * Constants.SECTOR_SIZE;
            if (levelTocStart == 0 || levelTocStart > isoReader.BaseStream.Length)
                throw new InvalidLevelTOCException($"Invalid level toc offset {levelTocStart:X8}");

            // get toc
            return isoReader.ReadBytes(levelTocStart, Constants.SECTOR_SIZE * 1);
        }

        private static void Unpack(BinaryReader isoReader, byte[] toc, int levelId, string outDir)
        {
            var isLevelMultiplayer = levelId >= 41;
            var wadDefs = new List<LevelWadDef>();

            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

            // Process toc
            using (var ms = new MemoryStream(toc, false))
            {
                using (var tocReader = new BinaryReader(ms))
                {
                    // skip header
                    tocReader.ReadBytes(4);
                    long baseOffset = (long)tocReader.ReadUInt32() * Constants.SECTOR_SIZE;
                    tocReader.ReadBytes(8);

                    // read wad offset and sizes
                    int i = 0;
                    while (tocReader.BaseStream.Position < tocReader.BaseStream.Length)
                    {
                        var wadOffset = baseOffset + tocReader.ReadUInt32() * Constants.SECTOR_SIZE;
                        var wadSize = tocReader.ReadInt32() * Constants.SECTOR_SIZE;

                        wadDefs.Add(new LevelWadDef() { Offset = wadOffset, Size = wadSize });
                        ++i;
                    }

                    // primary level wad is at index 1
                    UnpackLevelWad(isoReader, wadDefs[1], outDir);
                }
            }
        }

        private static void UnpackLevelWad(BinaryReader isoReader, LevelWadDef wadDef, string outLevelRootDir)
        {
            var outDir = Path.Combine(outLevelRootDir, "base");
            if (!Directory.Exists(outDir)) Directory.CreateDirectory(outDir);

            var wad = isoReader.ReadBytes(wadDef.Offset, wadDef.Size);
            File.WriteAllBytes(Path.Combine(outDir, "level.wad"), wad);

            UnpackLevelSprites.Unpack(isoReader, wadDef, outDir);
        }
    }
}
