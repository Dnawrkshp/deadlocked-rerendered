using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public static class ProcessorHelper
{
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsProcessorFeaturePresent(int processorFeature);

    public static bool ProcessorSupportsAVX2() => IsProcessorFeaturePresent(40); // PF_AVX2_INSTRUCTIONS_AVAILABLE
}
