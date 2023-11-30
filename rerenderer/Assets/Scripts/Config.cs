using Newtonsoft.Json;
using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Config
{
    public static readonly string CONFIG_PATH = "config.json";
    public static Config Singleton = null;
    public static readonly string[] INPUT_SOURCES = new[] { "Auto", "KBM", "Joystick" };

    public string IsoPath { get; set; }
    public bool ShowFPSCounter { get; set; } = true;
    public bool ShownFirstTimePopup { get; set; } = false;
    public int EmulatorBuildType { get; set; } = 0; // AUTO, AVX2, DEFAULT
    public bool DynamicSettingFrameSleepWait { get; set; } = true;
    public int InputSource { get; set; } = 0; // auto
    public bool AcceptInputWhenNotFocus { get; set; } = false;
    public bool DisableFrameInterpolation { get; set; } = false;
    public VideoConfig Video { get; set; } = new VideoConfig();
    public GraphicsConfig Graphics { get; set; } = new GraphicsConfig();
    public AudioConfig Audio { get; set; } = new AudioConfig();
    public KBMConfig KBM { get; set; } = new KBMConfig();
    public EmulatorConfig Emulator { get; set; } = new EmulatorConfig();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        Config config = new Config();
        try
        {
            if (File.Exists(CONFIG_PATH))
            {
                var configJson = File.ReadAllText(CONFIG_PATH);
                JsonConvert.PopulateObject(configJson, config);
            }
        }
        catch { }

        // apply video
        config.ApplyVideo();

        // reload emulator settings
        config.Emulator.Reload();

        // save
        config.Save();

        Singleton = config;
    }

    public void Reload()
    {
        try
        {
            if (File.Exists(CONFIG_PATH))
            {
                var configJson = File.ReadAllText(CONFIG_PATH);
                JsonConvert.PopulateObject(configJson, this);
            }
        }
        catch { }

        // reload emulator settings
        Emulator.Reload();
    }

    public bool Save()
    {
        try
        {
            var configJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(CONFIG_PATH, configJson);
            return true;
        }
        catch { return false; }
    }

    public void ApplyVideo()
    {
#if !UNITY_EDITOR
        var changed = false;

        // only set monitor for fullscreen options
        if (Video.ScreenMode == FullScreenMode.FullScreenWindow || Video.ScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            // get display (and correct)
            // move to monitor
            var displays = new List<DisplayInfo>();
            Screen.GetDisplayLayout(displays);
            var display = displays.ElementAtOrDefault(Video.Monitor);
            if (display.name != null)
            {
                Dispatcher.StartCoroutineOnMainThread(MoveWindow(display, null));
            }
            else
            {
                var activeDisplay = Display.displays.FirstOrDefault(x => x.active);
                Video.Monitor = Array.IndexOf(Display.displays, activeDisplay);
                changed = true;
            }
        }

        // Video
        QualitySettings.vSyncCount = Video.VSync ? 1 : 0;
        Application.targetFrameRate = Video.GetTargetFps();
        Screen.SetResolution(Video.Width, Video.Height, Video.ScreenMode, new RefreshRate() { numerator = (uint)Video.GetTargetFps(), denominator = 1 });
        
        // save changes
        if (changed)
            Save();
#endif
    }

    private System.Collections.IEnumerator MoveWindow(DisplayInfo display, Action callback)
    {
        var op = Screen.MoveMainWindowTo(display, Screen.mainWindowPosition);
        yield return op;

        Screen.SetResolution(Video.Width, Video.Height, Video.ScreenMode, new RefreshRate() { numerator = (uint)Video.GetTargetFps(), denominator = 1 });
        callback?.Invoke();
    }
}

public class VideoConfig
{
    public int Monitor { get; set; } = 0;
    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;
    public bool VSync { get; set; } = true;
    public int TargetFramerate { get; set; } = 60;
    public FullScreenMode ScreenMode { get; set; } = FullScreenMode.Windowed;

    public int GetTargetFps() => Mathf.Max(TargetFramerate, 60);
}

public class GraphicsConfig
{
    public int Quality { get; set; } = 2;
    public float RenderScale { get; set; } = 1;
}

public class AudioConfig
{
    public float MasterVolume { get; set; } = 1;
    public float MusicVolume { get; set; } = 1;
    public float EffectsVolume { get; set; } = 1;
    public float DialogVolume { get; set; } = 1;
}

public class EmulatorConfig
{
    private static readonly string PCSX2_INI = "inis/PCSX2.ini";
    private static readonly string PCSX2_UI_INI = "inis/PCSX2_ui.ini";
    private static readonly string PCSX2_VM_INI = "inis/PCSX2_vm.ini";
    private static readonly string PCSX2_USB_INI = "inis/USB.ini";
    private static readonly string PCSX2_HOSTS_INI = "inis/DEV9Hosts.ini";
    private static readonly string PCSX2_SPU_INI = "inis/SPU2.ini";

    private static readonly string DEFAULT_PCSX2_USB_INI = "../defaultinis/USB.ini";

    [JsonIgnore]
    public bool BiosValid { get; private set; } = false;
    public string BiosFolder { get; set; } = String.Empty;
    public string BiosFilename { get; set; } = String.Empty;
    public int AudioLatency { get; set; } = 3;
    public int AudioSync { get; set; }
    public int EECycleRate { get; set; }

    public EmulatorConfig()
    {
        // load settings
        Reload();
    }

    public void Reload()
    {
        ValidateInis();

        //var ui = ParseIni(PCSX2_UI_INI);
        //var vm = ParseIni(PCSX2_VM_INI);
        //var hosts = ParseIni(PCSX2_HOSTS_INI);
        //var spu = ParseIni(PCSX2_SPU_INI);

        //BiosFolder = ReadValue(ui, "Folders.Bios").Replace("\\\\", "\\");
        //BiosFilename = ReadValue(ui, "Filenames.BIOS");
        //EthApi = ReadValue(vm, "DEV9/Eth.EthApi");
        //EthDevice = ReadValue(vm, "DEV9/Eth.EthDevice");
        //EthInterceptDHCP = ReadValue(vm, "DEV9/Eth.InterceptDHCP")?.ToLower() == "enabled";
        //EthPS2Ip = ReadValue(vm, "DEV9/Eth.PS2IP");
        //EthPS2Mask = ReadValue(vm, "DEV9/Eth.Mask");
        //EthPS2Gateway = ReadValue(vm, "DEV9/Eth.Gateway");
        //EthPS2DNS1 = ReadValue(vm, "DEV9/Eth.DNS1");
        //EthPS2DNS2 = ReadValue(vm, "DEV9/Eth.DNS2");
        //EthPS2MaskAuto = ReadValue(vm, "DEV9/Eth.AutoMask")?.ToLower() == "enabled";
        //EthPS2GatewayAuto = ReadValue(vm, "DEV9/Eth.AutoGateway")?.ToLower() == "enabled";
        //EthPS2DNS1Auto = ReadValue(vm, "DEV9/Eth.ModeDNS1") == "Auto";
        //EthPS2DNS2Auto = ReadValue(vm, "DEV9/Eth.ModeDNS2") == "Auto";
        //HostDeadlockedDns = ReadValue(hosts, "Host0.Address");
        //AudioSync = int.Parse(ReadValue(spu, "OUTPUT.Synch_Mode"));
        //AudioLatency = int.Parse(ReadValue(spu, "OUTPUT.Latency"));

        BiosFolder = BiosFolder ?? "";
        BiosFilename = BiosFilename ?? "";

        var biosPath = Path.Combine(Constants.GetEmuPath(), BiosFolder, BiosFilename);
        BiosValid = File.Exists(biosPath);
    }

    public void Save()
    {
        SaveQt();
    }

    private void SaveQt()
    {
        ValidateInis();

        var pcsx2 = ParseIni(PCSX2_INI);

        if (pcsx2.Count > 0)
        {
            //WriteValue(pcsx2, "Folders.UseDefaultBios", "disabled");
            WriteValue(pcsx2, "Folders.Bios", BiosFolder.Replace("\\", "\\\\"));
            WriteValue(pcsx2, "Filenames.BIOS", BiosFilename);

            // write ethernet settings
            WriteValue(pcsx2, "DEV9/Eth.EthEnable", "false");
            
            // write base settings
            WriteValue(pcsx2, "EmuCore.EnableCheats", "false");
            WriteValue(pcsx2, "EmuCore.EnablePINE", "true");
            WriteValue(pcsx2, "EmuCore.EnableWideScreenPatches", "false");
            WriteValue(pcsx2, "EmuCore/GS.FrameLimitEnable", "false");
            WriteValue(pcsx2, "EmuCore/Speedhacks.EECycleRate", EECycleRate.ToString());

            // write audio settings
            WriteValue(pcsx2, "SPU2/Output.Latency", Mathf.Max(30, AudioLatency).ToString());
            WriteValue(pcsx2, "SPU2/Output.SynchMode", AudioSync.ToString());

            WriteIni(PCSX2_INI, pcsx2);
        }
    }

    public void SetBiosPath(string fullpath)
    {
        var emuPath = Constants.GetEmuPath();
        var fi = new FileInfo(fullpath);

        // use relative path if folder exists inside emuPath
        // otherwise use absolute path
        BiosFolder = Path.GetRelativePath(emuPath, fi.DirectoryName);
        if (BiosFolder.StartsWith(".."))
            BiosFolder = fi.DirectoryName;

        BiosFilename = fi.Name;
        BiosValid = fi.Exists;
    }

    private string ReadValue(List<KeyValuePair<string, string>> dict, string key)
    {
        return dict.FirstOrDefault(x => x.Key == key).Value;
    }

    private void WriteValue(List<KeyValuePair<string, string>> dict, string fullKey, string value)
    {
        var parts = fullKey.Split('.');
        var category = parts[0] + ".";
        var key = String.Join('.', parts.Skip(1));

        var i = dict.FindIndex(x => x.Key == fullKey);
        if (i < 0)
        {
            i = dict.FindLastIndex(x => x.Key.StartsWith(category));
            if (i < 0)
            {
                // add to end if full key and category don't exist yet
                dict.Add(new KeyValuePair<string, string>(fullKey, value));
            }
            else
            {
                // insert near same category
                dict.Insert(i+1, new KeyValuePair<string, string>(fullKey, value));
            }
        }
        else
        {
            // replace existing entry with ours
            dict[i] = new KeyValuePair<string, string>(fullKey, value);
        }
    }

    private List<KeyValuePair<string, string>> ParseIni(string path)
    {
        var dict = new List<KeyValuePair<string, string>>();

        var fullPath = Path.Combine(Constants.GetEmuPath(), path);

        if (File.Exists(fullPath))
        {
            var contents = File.ReadAllLines(fullPath);
            var category = "";

            foreach (var line in contents)
            {
                if (String.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    category = line.Substring(1, line.Length - 2);
                    continue;
                }

                var parts = line.Split('=');
                var key = parts[0].Trim();
                var value = String.Join('=', parts.Skip(1));
                dict.Add(new KeyValuePair<string, string>($"{category}.{key}", value));
            }
        }

        return dict;
    }

    private void WriteIni(string path, List<KeyValuePair<string, string>> dict)
    {
        var fullPath = Path.Combine(Constants.GetEmuPath(), path);
        string lastCategory = "";
        string ini = "";
        foreach (var kvp in dict)
        {
            var parts = kvp.Key.Split('.');
            var category = parts[0];
            var key = String.Join('.', parts.Skip(1));
            var value = kvp.Value;

            if (category != lastCategory)
            {
                ini += $"[{category}]\n";
                lastCategory = category;
            }

            ini += $"{key}={value}\n";
        }

        File.WriteAllText(fullPath, ini);
    }
    
    private void ValidateInis()
    {
        // check if emulater ini files exist
        // if not, copy the default ones over
        var defaultinisPath = Path.Combine(Application.dataPath, "defaultinis", "qt");
        if (Directory.Exists(defaultinisPath))
        {
            var emuInisPath = Path.Combine(Constants.GetEmuPath(), "inis");
            if (!Directory.Exists(emuInisPath))
                Directory.CreateDirectory(emuInisPath);

            var iniFiles = Directory.GetFiles(defaultinisPath, "*.ini");
            foreach (var ini in iniFiles)
            {
                var emuIniPath = Path.Combine(emuInisPath, Path.GetFileName(ini));
                if (!File.Exists(emuIniPath))
                    File.Copy(ini, emuIniPath);
            }
        }
    }
}

public class KBMConfig
{
    public float MouseSensitivityX { get; set; } = 1;
    public float MouseSensitivityY { get; set; } = 1;
    public bool MouseInvertY { get; set; } = false;
}
