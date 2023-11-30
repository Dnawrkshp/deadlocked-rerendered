using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class Init : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // validate editor install
#if UNITY_EDITOR
        InstallEditorEmu();
#endif

        // spawn gamemanager
        if (!GameManager.Singleton)
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/Manager"));
        }

        Destroy(this.gameObject);
    }

#if UNITY_EDITOR
    void InstallEditorEmu()
    {
        var emuPath = Constants.GetEmuPath();
        if (Directory.Exists(emuPath)) return;

        // copy from plugins
        var srcPath = Path.Combine(Application.dataPath, "Plugins", "PCSX2", "qt");
        var srcBinariesPath = Path.Combine(srcPath, "win64") + "\\";
        var srcSharedPath = Path.Combine(srcPath, "shared") + "\\";
        var srcDefaultInisPath = Path.Combine(srcPath, "defaultinis") + "\\";
        var outDir = emuPath;
        var outInisDir = Path.Combine(outDir, "inis");

        Directory.CreateDirectory(outDir);
        Directory.CreateDirectory(outInisDir);
        FileHelper.CopyAll(srcBinariesPath, outDir);
        FileHelper.CopyAll(srcSharedPath, outDir);
        FileHelper.CopyAll(srcDefaultInisPath, outInisDir);
    }
#endif
}
