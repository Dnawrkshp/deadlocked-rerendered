using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unpacker.cli
{
    [Verb("unpack-levels")]
    class UnpackLevelsOp
    {
        [Option('i', "iso", Required = true, HelpText = "Path to iso.")]
        public string? IsoPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output folder.")]
        public string? OutFolder { get; set; }

        public OpResultCode Run()
        {
            if (IsoPath == null) return OpResultCode.GENERIC_ERROR;
            if (OutFolder == null) return OpResultCode.GENERIC_ERROR;

            for (int i = 0; i < 75; ++i)
            {
                Console.WriteLine($"unpacking level {i}");
                try
                {
                    UnpackLevel.Unpack(IsoPath, i, Path.Combine(OutFolder, "levels", $"{i}"));
                }
                catch (UnpackLevel.InvalidLevelTOCException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return OpResultCode.GENERIC_ERROR;
                }
            }

            return OpResultCode.SUCCESS;
        }

    }
}
