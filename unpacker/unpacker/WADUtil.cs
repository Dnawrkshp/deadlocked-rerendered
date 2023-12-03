using System.Diagnostics;

namespace unpacker
{
    public static class WADUtil
    {
        private static void run(string cmd)
        {
            Process? p = Process.Start(new ProcessStartInfo()
            {
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = "resources/wrenchbuild.exe",
                Arguments = cmd
            });

            p?.WaitForExit();
        }

        public static bool VerifyMagic(byte[] magic)
        {
            if (magic.Length < 3)
                return false;

            return magic[0] == 'W' && magic[1] == 'A' && magic[2] == 'D';
        }

        public static byte[]? Decompress(byte[] src, Action<long, long, long> callback = null)
        {
            byte[]? result = null;
            var inPath = Path.GetTempFileName();
            var outPath = Path.GetTempFileName();

            if (File.Exists(outPath)) File.Decrypt(outPath);
            File.WriteAllBytes(inPath, src);
            Unpack(inPath, outPath);
            
            if (File.Exists(outPath)) result = File.ReadAllBytes(outPath);
            return result;
        }

        public static void Unpack(string infile, string outfile)
        {
            UnpackSegment(infile, outfile);
        }

        public static void UnpackSegment(string infile, string outfile)
        {
            run($"decompress \"{infile}\" -o \"{outfile}\"");
        }

    }
}
