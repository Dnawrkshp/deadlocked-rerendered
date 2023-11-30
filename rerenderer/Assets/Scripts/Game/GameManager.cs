using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; } = null;

    public event Action<IGameCommand> OnCommand;
    public event Action OnPreTick;
    public event Action OnTick;
    public event Action OnPostTick;

    public List<IGameCommand> IncomingCommands { get; set; } = new List<IGameCommand>();
    public List<IGameCommand> OutgoingCommands { get; set; } = new List<IGameCommand>();

    public GameObject TempMobyRootGameObject { get; private set; }

    private System.Diagnostics.Stopwatch _sw = System.Diagnostics.Stopwatch.StartNew();

    private void Awake()
    {
        Singleton = this;

        EmuInterop.OnPreTick -= EmuInterop_OnPreTick;
        EmuInterop.OnPreTick += EmuInterop_OnPreTick;
        RenderPipelineManager.beginCameraRendering -= RenderPipelineManager_beginCameraRendering;
        RenderPipelineManager.beginCameraRendering += RenderPipelineManager_beginCameraRendering;
    }

    private void RenderPipelineManager_beginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
        arg2.ResetProjectionMatrix();
        arg2.projectionMatrix *= Matrix4x4.Scale(Constants.POST_STRETCH_FACTOR_4_3);
    }

    private void EmuInterop_OnPreTick()
    {
        try
        {
            if (!TempMobyRootGameObject)
                TempMobyRootGameObject = new GameObject("temp");


            DebugStats.ResetStats();

            OnPreTick?.Invoke();
            if (PostInterop.Ready)
                ProcessIncomingCommands();

            //var gameManagerMobyUpdateStartMs = _sw.Elapsed.TotalMilliseconds;
            //moby update
            //DebugStats.MobyAnimatorUpdateTimeMs = _sw.Elapsed.TotalMilliseconds - gameManagerMobyUpdateStartMs;

            OnTick?.Invoke();
            OnPostTick?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void ProcessIncomingCommands()
    {
        foreach (var incomingCmd in IncomingCommands)
        {
            if (incomingCmd is GameCommandMobySpawned mobySpawnedCmd)
            {
                Debug.Log($"AutoSpawn: Skipping new moby {mobySpawnedCmd.MobyClass:X4} at {mobySpawnedCmd.MobyInstancePtr:X8}");
            }

            OnCommand?.Invoke(incomingCmd);
        }

        IncomingCommands.Clear();
    }

}
