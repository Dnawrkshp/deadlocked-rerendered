using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class PostInterop : MonoBehaviour
{
    public static bool Ready = false;
    public static bool Failed = false;
    public static bool Active = false;
    public static bool Connecting => RetryConnectPine;
    public static bool PatchInitialized = false;
    private static bool SentPatch = false;
    private static float TimeLastRestarted = 0;
    private static bool RetryConnectPine = false;

    byte[] commandBuffer = new byte[0x1000];

    private void Awake()
    {
        EmuInterop.OnPostTick -= EmuInterop_OnPostTick;
        EmuInterop.OnPostTick += EmuInterop_OnPostTick;
    }

    private void Start()
    {
        SceneManager.LoadScene("Main");
        //StartEmulator();
    }

    private void Update()
    {
        //if (GameController.UsingGameRender)
        //{
        //    Application.targetFrameRate = 60;
        //    QualitySettings.vSyncCount = 0;
        //}
        //else
        //{
        //    Application.targetFrameRate = Config.Singleton.Video.GetTargetFps();
        //    QualitySettings.vSyncCount = Config.Singleton.Video.VSync ? 1 : 0;
        //}
    }

    private void OnApplicationQuit()
    {
        EmuInterop.Dispose();
        EmuInterop.KillEmu();
    }

    private void FixedUpdate()
    {
        if (RetryConnectPine)
        {
            if (!EmuInterop.Connecting && !EmuInterop.Connected)
            {
                if ((Time.unscaledTime - TimeLastRestarted) > Constants.EMU_STARTUP_TIMEOUT_SECONDS)
                {
                    // timedout
                    Failed = true;
                    RetryConnectPine = false;
                }
                else
                {
                    EmuInterop.StartNewPine(false);
                }
            }
            else if (EmuInterop.ConnectedAndGameStarted)
            {
#if !UNITY_EDITOR
                    //EmuInterop.Stop();
#endif
                RetryConnectPine = false;
                EmuInterop.Resume();
                PostInterop.Active = true;
            }
        }
    }

    private void EmuInterop_OnPostTick()
    {
        if (!Active)
            Ready = false;

        if (Active)
        {
            // wait for restart
            if ((Time.unscaledTime - TimeLastRestarted) < 1f)
                return;

            if (!EmuInterop.ConnectedAndGameStarted)
            {
                Active = false;
                Ready = false;
                return;
            }

            if (!SentPatch)
            {
                SendPatch();
            }
            else
            {
                // ensure that the patch is still installed
                // in case the user exits multiplayer
                if (EmuInterop.ReadInt32(Constants.PatchHookAddress) != Constants.PatchHookValue)
                {
                    SentPatch = false;
                    Ready = false;
                    PatchInitialized = false;
                    return;
                }
            }

            if (!Ready)
            {
                // gives the patch time to initialize before we start sending commands over
                if (SentPatch && !PatchInitialized && EmuInterop.ReadInt32(Constants.ConfigAddress) > 0)
                {
                    PatchInitialized = true;
                    Ready = true;

                    // send dynamic settings
                    EmuInterop.SetFrameSleepWait(Config.Singleton.DynamicSettingFrameSleepWait).TryGetResult(out _);
                }
            }
        }

        if (Ready)
        {
            // write out commands
            using (var ms = new MemoryStream(commandBuffer, true))
            {
                using (var writer = new BinaryWriter(ms))
                {
                    foreach (var cmd in GameManager.Singleton.OutgoingCommands)
                    {
                        writer.Write((byte)cmd.Id);
                        cmd.Serialize(writer);
                    }
                }
            }

            // write bytes
            EmuInterop.WriteBytes(Constants.CmdOutAddress, commandBuffer);
            GameManager.Singleton.OutgoingCommands.Clear();
        }
    }

    public static void StartEmulator()
    {
        RetryConnectPine = false;
        var spawnEmu = EmuInterop.SpawnEmu();
        if (spawnEmu == false)
        {
            RetryConnectPine = true;
            PostInterop.Active = false;
        }
        else if (spawnEmu == true)
        {
            PostInterop.Active = true;
        }
        else
        {
            PostInterop.Active = false;
        }

        Failed = false;
        PatchInitialized = false;
        SentPatch = false;
        TimeLastRestarted = Time.unscaledTime;
    }

    void SendPatch()
    {
        // must be running
        //if (EmuInterop.GetStatus().TryGetResult(out var status) && status.Code != IPCStatusCode.Running)
        //    return;

        if (!RCHelper.GetRunningGame().HasValue)
            return;

        // wait for hook to be correct value
        var patchHookValue = EmuInterop.ReadInt32(Constants.PatchHookAddress);
        if (patchHookValue != Constants.PatchHookOriginalValue && patchHookValue != Constants.PatchHookValue)
            return;

        // send patch
#if UNITY_EDITOR
        var path = Path.Combine(Application.dataPath, "Resources", "Patch", "patch-ntsc.bin");
#else
        var path = Path.Combine(Application.dataPath, "patch-ntsc.bin");
#endif

        if (File.Exists(path))
        {
            // write patch
            EmuInterop.WriteInt32(Constants.PatchHookAddress, (int)Constants.PatchHookOriginalValue);
            EmuInterop.WriteBytes(Constants.PatchAddress, File.ReadAllBytes(path));
            EmuInterop.WriteInt32(Constants.PatchHookAddress, (int)Constants.PatchHookValue);
        }

        SentPatch = true;
    }
}
