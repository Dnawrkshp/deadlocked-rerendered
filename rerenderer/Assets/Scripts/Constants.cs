using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

public static class Constants
{
    public static string VERSION = "0.0.1";

    public static readonly uint PatchAddress = 0x02010000;
    public static readonly uint ConfigAddress = PatchAddress + 8;
    public static readonly uint CmdInAddress = 0x02080000;
    public static readonly uint CmdOutAddress = 0x02090000;
    public static readonly uint PatchHookAddress = 0x001270d0;
    public static readonly uint PatchHookValue = 0x0C000000 | (PatchAddress >> 2);
    public static readonly uint PatchHookOriginalValue = 0x0C049BC4;

    public static readonly Vector3 POST_STRETCH_FACTOR_4_3 = new Vector3(1, 1 / 0.88f, 1);

    public static readonly float SCREEN_WIDTH = 512f;
    public static readonly float SCREEN_HEIGHT = 448f;

    public static readonly float UI_SCREEN_WIDTH = 476f;
    public static readonly float UI_SCREEN_HEIGHT = 416f;

    public static readonly float EMU_STARTUP_TIMEOUT_SECONDS = 20f;

    public static string GetEmuPath()
    {
        var emuFolderName = $"emuqt";

#if UNITY_EDITOR
        var emuPath = Path.Combine(Environment.CurrentDirectory, emuFolderName);
#else
        var emuPath = Path.Combine(Application.dataPath, emuFolderName);
#endif

        return emuPath;
    }

    public static string GetFaqFilePath()
    {
#if UNITY_EDITOR
        var faqFilePath = Path.Combine(Environment.CurrentDirectory, "faq.json");
#else
        var faqFilePath = Path.Combine(Application.dataPath, "faq.json");
#endif

        return faqFilePath;
    }
}
