using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Singleton { get; private set; } = null;

    public static PadData RawPad { get; } = new PadData();
    public static Texture2D GameRender = null;
    public static bool UsingGameRender = false;
    public static bool HideScene = false;
    public static bool HideTemp = false;
    public static bool HasInitialLoaded = false;
    public static bool ShowPCSX2GUI = false;
    public static bool WriteCosmeticsNextTick = true;

    private static System.Diagnostics.Stopwatch _sw = System.Diagnostics.Stopwatch.StartNew();

    public UnityEngine.UI.RawImage emuRenderBgComponent;
    public UnityEngine.UI.RawImage emuRenderComponent;
    public UnityEngine.UI.RawImage blurOverlayComponent;
    public GameObject debugStatsPanel;

    private RectTransform canvasRectTransform;

    private byte[] frameBuffer = new byte[512 * 448 * 4];
    private byte[] padBuffer = new byte[32];

    private GameManager gameManager;
    private PlayerInput playerInput;
    private bool useRenderInGame = false;
    private bool forceRenderInGame = false;
    private bool hideHud = false;
    private Camera mainCamera = null;
    private GameObject tempGo = null;
    private int lastInputSource = -1;
    private int homeButtonCooldown = 0;


#if UNITY_EDITOR
    private static bool overrideRenderingInEditor = true;
#endif

#if UNITY_EDITOR
    private bool disableRenderingInGame = overrideRenderingInEditor;
#else
    private bool disableRenderingInGame = true;
#endif

    private void Awake()
    {
        Singleton = this;

        EmuInterop.OnTick -= EmuInterop_OnTick;
        EmuInterop.OnTick += EmuInterop_OnTick;

        gameManager = GetComponent<GameManager>();
        playerInput = GetComponent<PlayerInput>();
        canvasRectTransform = emuRenderBgComponent.canvas.GetComponent<RectTransform>();

        if (GameRender == null)
        {
            GameRender = new Texture2D(512, 448, TextureFormat.BGRA32, mipChain: false);
            GameRender.wrapMode = TextureWrapMode.Clamp;
        }

        DontDestroyOnLoad(this.gameObject);

        gameManager.OnCommand -= GameManager_OnCommand;
        gameManager.OnCommand += GameManager_OnCommand;

        ShowPCSX2GUI = false;
        var args = Environment.GetCommandLineArgs();
        if (args != null && args.Contains("showgui"))
            ShowPCSX2GUI = true;

#if UNITY_EDITOR
        ShowPCSX2GUI = true;
#endif

        debugStatsPanel.SetActive(DebugStats.ShowStats);
    }

    //private void Update()
    //{
    //    ProcessLocalInput();
    //}

    private void FixedUpdate()
    {
        if (!EmuInterop.ConnectedAndGameStarted)
        {
            emuRenderBgComponent.enabled = false;
            emuRenderComponent.enabled = false;
            UIManager.RenderGameUI = false;
            UIManager.RenderMenu = true;
            UsingGameRender = false;
            blurOverlayComponent.enabled = UIManager.RenderMenu;
            HasInitialLoaded = false;
        }

        if (UnityEngine.InputSystem.Keyboard.current.f11Key.wasPressedThisFrame)
        {
            useRenderInGame = !useRenderInGame;
            SnackManager.AddSnack($"DZO Render {(useRenderInGame ? "Off" : "On")}");
        }

        if (UnityEngine.InputSystem.Keyboard.current.f10Key.wasPressedThisFrame)
        {
            hideHud = !hideHud;
            SnackManager.AddSnack($"HUD {(hideHud ? "Off" : "On")}");
        }

        if (UnityEngine.InputSystem.Keyboard.current.f12Key.wasPressedThisFrame)
        {
            DebugStats.ShowStats = !DebugStats.ShowStats;
            debugStatsPanel.SetActive(DebugStats.ShowStats);
            SnackManager.AddSnack($"Show Debug Stats set to {(DebugStats.ShowStats ? "On" : "Off")}");
        }

        if (UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Config.Singleton.InputSource = (Config.Singleton.InputSource + 1) % 3;
            SnackManager.AddSnack($"Input Source set to {Config.INPUT_SOURCES[Config.Singleton.InputSource]}");
        }

#if UNITY_EDITOR || DEBUG

        if (UnityEngine.InputSystem.Keyboard.current.f9Key.wasPressedThisFrame)
        {
            disableRenderingInGame = !disableRenderingInGame;
            SnackManager.AddSnack($"PCSX2 Scene Render {(disableRenderingInGame ? "Off" : "On")}");
        }

        if (UnityEngine.InputSystem.Keyboard.current.f8Key.wasPressedThisFrame)
        {
            HideScene = !HideScene;
            SnackManager.AddSnack($"DZO Scene Render {(HideScene ? "Off" : "On")}");
        }

        if (UnityEngine.InputSystem.Keyboard.current.f7Key.wasPressedThisFrame)
        {
            HideTemp = !HideTemp;

            if (!tempGo)
                tempGo = GameObject.Find("temp");

            if (tempGo)
                tempGo.SetActive(!HideTemp);

            SnackManager.AddSnack($"DZO Moby Render {(HideTemp ? "Off" : "On")}");
        }

#endif
    }

    private void EmuInterop_OnTick()
    {
        UsingGameRender = false;
        if (EmuInterop.ConnectedAndGameStarted)
        {
            // wait for emu to load all the way to the multiplayer screen
            // then hide the menu and continue
            //if (!HasInitialLoaded && PostInterop.PatchInitialized && RCHelper.IsSceneLoading())
            //    return;
            //if (!PostInterop.Ready && RCHelper.IsDeadlocked())
            //    return;
            if (!HasInitialLoaded)
                UIManager.RenderMenu = false;

            var useEmuRender = forceRenderInGame || useRenderInGame;

            HasInitialLoaded = true;
            SendPad();

            if (!mainCamera)
                mainCamera = Camera.main;

            if (true)
            {
                UpdateRender();

                // render
                emuRenderBgComponent.enabled = true;
                emuRenderComponent.enabled = true;
                emuRenderComponent.texture = GameRender;
                UIManager.RenderGameUI = true;
            }

            // indicate to patch to disable rendering if we're not using game render
            if (ShowPCSX2GUI)
                EmuInterop.WriteInt32(Constants.ConfigAddress + 4 + 0x20 + 8, (disableRenderingInGame) ? 1 : 0);
            else
                EmuInterop.WriteInt32(Constants.ConfigAddress + 4 + 0x20 + 8, (!UsingGameRender) ? 1 : 0);

            // write audio settings
            if (RCHelper.IsRunningDeadlocked())
            {
                EmuInterop.WriteInt32(0x00171D44, (int)(0x400 * Config.Singleton.Audio.MasterVolume * Config.Singleton.Audio.MusicVolume));
                EmuInterop.WriteInt32(0x00171D48, (int)(0x400 * Config.Singleton.Audio.MasterVolume * Config.Singleton.Audio.EffectsVolume));
                EmuInterop.WriteInt32(0x00171D4C, (int)(0x400 * Config.Singleton.Audio.MasterVolume * Config.Singleton.Audio.DialogVolume)); //.TryGetResult(out _);
            }
        }


        blurOverlayComponent.enabled = UIManager.RenderMenu;
        var ratio = canvasRectTransform.sizeDelta.y / GameRender.height;
        blurOverlayComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UsingGameRender ? ratio * GameRender.width : canvasRectTransform.sizeDelta.x);
        blurOverlayComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UsingGameRender ? ratio * GameRender.height : canvasRectTransform.sizeDelta.y);
    }

    private void UpdateRender()
    {
        // update emu rendering settings
        EmuInterop.SetDisableRendering(false);

        var framePromise = EmuInterop.GetFrameBuffer(frameBuffer);
        if (framePromise.TryGetResult(out var result) && result)
        {
            GameRender.LoadRawTextureData(frameBuffer);
            GameRender.Apply();
            UsingGameRender = true;
        }
    }

    static readonly byte[] joystickNoAcc = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    static readonly byte[] joystickDefaultAccYaw = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x51, 0x77, 0xD6, 0x3D, 0x51, 0x77, 0x56, 0x3D, 0x89, 0xC3, 0x64, 0x3B, 0x98, 0x2E, 0xFE, 0x3A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3F };
    static readonly byte[] joystickDefaultAccPitch = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x36, 0xFA, 0x0E, 0x3D, 0xF2, 0xA2, 0xBE, 0x3A, 0x9A, 0x99, 0x19, 0x3F, 0xFA, 0x7E, 0x2A, 0x3F, 0x9A, 0x99, 0x59, 0x3F, 0xC2, 0xB8, 0xB2, 0x3E };

    private void SendPad()
    {
        // send camera delta
        if (RawPad.IsGameKBM)
        {
            EmuInterop.WriteFloat(Constants.ConfigAddress + 4 + 0x20 + 0, (RawPad.LookDelta.x * Config.Singleton.KBM.MouseSensitivityX * -0.001f));
            EmuInterop.WriteFloat(Constants.ConfigAddress + 4 + 0x20 + 4, (RawPad.LookDelta.y * Config.Singleton.KBM.MouseSensitivityY * (Config.Singleton.KBM.MouseInvertY ? -1 : 1) * -0.001f));
        }
        
        // send gamepad
        RawPad.WriteBytes(padBuffer);
        padBuffer[31] = 0x5a;
        EmuInterop.WriteBytes(Constants.ConfigAddress + 4, padBuffer);

        // reset pad
        RawPad.LookDelta = Vector2.zero;
        RawPad.LeftTrigger = RawPad.RightTrigger = 0f;
        RawPad.LeftHorizontal = RawPad.LeftVertical = RawPad.RightHorizontal = RawPad.RightVertical = 0;
        RawPad.Buttons = PadButtons.None;

        if (!RCHelper.IsRunningDeadlocked())
            EmuInterop.SetPad(0, 0, padBuffer);
    }

    private void GameManager_OnCommand(IGameCommand cmd)
    {

    }

    private void SceneLoadOp_completed(AsyncOperation obj)
    {

    }

    private Vector2 RemapJoystickInput(Vector3 value)
    {
        var move = value;
        value.x = Mathf.Clamp(move.x + move.x * Mathf.Abs(move.y), -1, 1);
        value.y = Mathf.Clamp(move.y + move.y * Mathf.Abs(move.x), -1, 1);
        return value;
    }

    public void ProcessLocalInput()
    {
        if (playerInput == null || Config.Singleton == null)
            return;

        InputSystem.settings.backgroundBehavior = Config.Singleton.AcceptInputWhenNotFocus ? InputSettings.BackgroundBehavior.IgnoreFocus : InputSettings.BackgroundBehavior.ResetAndDisableNonBackgroundDevices;

        if (UIManager.RenderMenu && Config.Singleton.InputSource == 2)
        {
            // always enable kbm support in menus
            //foreach (var controller in ReInput.controllers.Controllers)
            //    controller.enabled = true;

            lastInputSource = -1;
        }
        else if (lastInputSource != Config.Singleton.InputSource)
        {
            lastInputSource = Config.Singleton.InputSource;

            switch (Config.Singleton.InputSource)
            {
                case 0: // auto
                    {
                        playerInput.SwitchCurrentControlScheme("All");
                        break;
                    }
                case 1: // kbm
                    {
                        playerInput.SwitchCurrentControlScheme("Keyboard&Mouse");
                        break;
                    }
                case 2: // joystick
                    {
                        playerInput.SwitchCurrentControlScheme("Gamepad");
                        break;
                    }
            }
        }

        // handle inputs
        var isLookKBM = playerInput.actions["Look"].activeControl?.name == "Mouse";
        var isNavKBM = playerInput.actions["Look"].activeControl?.name == "Mouse"
            || playerInput.actions["Move"].activeControl?.name == "Keyboard"
            ;
        var isLookJoystick = playerInput.actions["Look"].activeControl?.name == "Gamepad";
        var isNavJoystick = playerInput.actions["Look"].activeControl?.name == "Gamepad"
            || playerInput.actions["Move"].activeControl?.name == "Gamepad"
            ;

        if (isLookKBM && !isLookJoystick)
            RawPad.IsGameKBM = true;
        else if (isLookJoystick && !isLookKBM)
            RawPad.IsGameKBM = false;

        if (isNavKBM && !isNavJoystick)
            RawPad.IsNavKBM = true;
        else if (isNavJoystick && !isNavKBM)
            RawPad.IsNavKBM = false;

        // get inputs
        Vector2 move = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector2 look = playerInput.actions["Look"].ReadValue<Vector2>();

        RawPad.Buttons |= playerInput.actions["Up"].IsPressed() ? PadButtons.Up : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Down"].IsPressed() ? PadButtons.Down : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Left"].IsPressed() ? PadButtons.Left : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Right"].IsPressed() ? PadButtons.Right : PadButtons.None;

        RawPad.Buttons |= playerInput.actions["Cross"].IsPressed() ? PadButtons.Cross : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Circle"].IsPressed() ? PadButtons.Circle : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Square"].IsPressed() ? PadButtons.Square : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Triangle"].IsPressed() ? PadButtons.Triangle : PadButtons.None;

        RawPad.Buttons |= playerInput.actions["L1"].IsPressed() ? PadButtons.L1 : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["R1"].IsPressed() ? PadButtons.R1 : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["L2"].IsPressed() ? PadButtons.L2 : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["R2"].IsPressed() ? PadButtons.R2 : PadButtons.None;

        RawPad.Buttons |= playerInput.actions["Start"].IsPressed() ? PadButtons.Start : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["Select"].IsPressed() ? PadButtons.Select : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["L3"].IsPressed() ? PadButtons.L3 : PadButtons.None;
        RawPad.Buttons |= playerInput.actions["R3"].IsPressed() ? PadButtons.R3 : PadButtons.None;

        if (RawPad.IsGameKBM)
        {
            RawPad.LookDelta += new Vector2(look.x * Config.Singleton.KBM.MouseSensitivityX, look.y * Config.Singleton.KBM.MouseSensitivityY);
            look = Vector2.zero;
        }

        if (UIManager.RenderMenu)
        {
            Cursor.lockState = Config.Singleton.Video.ScreenMode == FullScreenMode.ExclusiveFullScreen ? CursorLockMode.Confined : CursorLockMode.None;
            RawPad.Buttons = PadButtons.None;
            RawPad.LeftTrigger = RawPad.RightTrigger = 0;
            RawPad.LookDelta = look = move = Vector2.zero;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // invert
        move.y = -move.y;
        look.y = -look.y;

        // write inputs
        RawPad.LeftHorizontal = move.x;
        RawPad.LeftVertical = move.y;
        RawPad.RightHorizontal = look.x;
        RawPad.RightVertical = look.y;

        // decrement home button cooldown
        if (homeButtonCooldown > 0) --homeButtonCooldown;

        // if cooldown is 0
        // and home button or escape key is pressed
        // trigger
        if (homeButtonCooldown == 0 && UIManager.Singleton && (playerInput.actions["Home"].WasPressedThisFrame() || UnityEngine.InputSystem.Keyboard.current.escapeKey.wasReleasedThisFrame))
        {
            homeButtonCooldown = 5;
            //UIManager.Singleton.MenuScreen.OnHomeButtonPressed();
        }
    }

    private void SpawnHeroes()
    {
        //var heroPrefab = Resources.Load<GameObject>("Shared/Hero");

        //if (!RCHelper.IsInGame())
        //    return;

        //// spawn players
        //var playerArray = EmuInterop.ReadBytes(0x00344C38, 10 * 4);
        //if (playerArray != null)
        //{
        //    for (int i = 0; i < 10; ++i)
        //    {
        //        var playerPtr = playerArray.Value.ToUInt32(i * 4);
        //        if (playerPtr > 0)
        //        {
        //            if (!GameManager.Singleton.Heroes.Any(x => x.Address == playerPtr))
        //            {
        //                var heroGo = Instantiate(heroPrefab);
        //                var heroLink = heroGo.GetComponent<HeroLink>();

        //                heroLink.IsLocal = playerPtr == 0x00347AA0;
        //                heroLink.Id = i;
        //                heroLink.Address = playerPtr;
        //            }
        //        }
        //    }
        //}
    }

}
