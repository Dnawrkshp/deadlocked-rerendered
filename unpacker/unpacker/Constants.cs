using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unpacker
{
    public static class Constants
    {
        public const int SECTOR_SIZE = 0x800;

        public const string FILE_EXTENSION_PNG = ".png";

        // Store all specific DL related constants here
        public struct DL
        {
            public const int LEVEL_WAD_TOC = 0x200C38;
            
            public const uint LEVEL_SPRITE_HEADER_OFFSET = 0x20;
            public const uint LEVEL_SPRITE_WAD1_OFFSET = 0x28;
            public const uint LEVEL_SPRITE_WAD2_OFFSET = 0x30;
        }

    }
}
