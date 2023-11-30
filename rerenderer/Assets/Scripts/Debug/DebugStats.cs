using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DebugStats : MonoBehaviour
{

    #region Stats

    public static bool ShowStats { get; set; } = false;

    // General
    public static double EMUTimeMs;

    // Interop
    public static double EMUCacheReadTimeMs;
    public static double EMUInteropTickTimeMs;
    public static long EMUWaitForFrameSkips;
    public static long EMUWaitForFrameSkipsLastSecond;

    // Moby
    public static int MobyAnimatorCount;
    public static int MobyAnimatorsDrawn;
    public static double MobyAnimatorUpdateTimeMs;
    public static int MobyAnimatorJointsInterpolated;
    public static double MobyAnimatorJointsInterpolatedTimeMs;

    public static void ResetStats()
    {
        MobyAnimatorCount = 0;
        MobyAnimatorsDrawn = 0;
    }

    #endregion

    #region Stat UI Rows

    enum DebugStatRowType
    {
        HEADER,
        LABEL_AND_VALUE
    }

    class DebugStatRow
    {
        public DebugStatRowType Type;
        public string Label;
        public Func<string> GetValue;
        public float StallUpdateForSeconds;

        public List<Text> TextComponents = new List<Text>();
        public float StallUntil;
    }

    static readonly List<DebugStatRow> Stats = new List<DebugStatRow>()
    {
        new DebugStatRow() { Type = DebugStatRowType.HEADER, Label = "General" },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " CPU", GetValue = () => { UpdateFrameTimings(); return $"{FrameTimings[0].cpuFrameTime:0.0} ms"; }, StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = "  Main Thread", GetValue = () => $"{FrameTimings[0].cpuMainThreadFrameTime:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = "  Present Wait", GetValue = () => $"{FrameTimings[0].cpuMainThreadPresentWaitTime:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = "  Render Thread", GetValue = () => $"{FrameTimings[0].cpuRenderThreadFrameTime:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " GPU", GetValue = () => $"{FrameTimings[0].gpuFrameTime:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " EMU", GetValue = () => $"{EMUTimeMs:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.HEADER, Label = "EMU Interop" },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Cache Read Time", GetValue = () => $"{EMUCacheReadTimeMs:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Tick Time", GetValue = () => $"{EMUInteropTickTimeMs:0.0} ms", StallUpdateForSeconds = 0.1f  },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Total Wait For Frame Skips", GetValue = () => EMUWaitForFrameSkips.ToString() },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Wait For Frame Skips", GetValue = () => EMUWaitForFrameSkipsLastSecond.ToString() },
        new DebugStatRow() { Type = DebugStatRowType.HEADER, Label = "Moby" },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Total", GetValue = () => MobyAnimatorCount.ToString() },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Drawn", GetValue = () => MobyAnimatorsDrawn.ToString() },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Update Time",  GetValue = () => $"{MobyAnimatorUpdateTimeMs:0.0} ms", StallUpdateForSeconds = 0.1f },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Joints Interp", GetValue = () => MobyAnimatorJointsInterpolated.ToString() },
        new DebugStatRow() { Type = DebugStatRowType.LABEL_AND_VALUE, Label = " Joints Interp Time", GetValue = () => $"{MobyAnimatorJointsInterpolatedTimeMs:0.0} ms", StallUpdateForSeconds = 0.1f },
    };

    static FrameTiming[] FrameTimings = new FrameTiming[1];
    static bool _hasUpdatedFrameTimingsThisFrame = false;

    #endregion

    public Font Font;

    private float _timeLastUpdatedWaitForFrameSkipsLastSecond;
    private long _lastWaitForFrameSkips;

    private void Awake()
    {
        BuildUI();
    }

    private void Update()
    {
        // update wait for frame skips
        if (Time.time >= _timeLastUpdatedWaitForFrameSkipsLastSecond)
        {
            EMUWaitForFrameSkipsLastSecond = EMUWaitForFrameSkips - _lastWaitForFrameSkips;
            _lastWaitForFrameSkips = EMUWaitForFrameSkips;
            _timeLastUpdatedWaitForFrameSkipsLastSecond = Time.time + 1f;
        }

        foreach (var stat in Stats)
        {
            switch (stat.Type)
            {
                case DebugStatRowType.LABEL_AND_VALUE:
                    {
                        if (Time.time >= stat.StallUntil)
                        {
                            stat.TextComponents[1].text = stat.GetValue();
                            stat.StallUntil = Time.time + stat.StallUpdateForSeconds;
                        }
                        break;
                    }
            }
        }

        _hasUpdatedFrameTimingsThisFrame = false;
    }

    private static void UpdateFrameTimings()
    {
        if (_hasUpdatedFrameTimingsThisFrame) return;

        FrameTimingManager.CaptureFrameTimings();
        FrameTimingManager.GetLatestTimings((uint)FrameTimings.Length, FrameTimings);
        _hasUpdatedFrameTimingsThisFrame = true;
    }

    private void BuildUI()
    {
        foreach (var stat in Stats)
        {
            switch (stat.Type)
            {
                case DebugStatRowType.HEADER:
                    {
                        var rowGo = new GameObject(stat.Label);
                        rowGo.transform.SetParent(this.transform, false);

                        // add label
                        var text = CreateTextComponent(rowGo, stat.Label);
                        text.fontSize = 16;

                        stat.TextComponents.Add(text);
                        break;
                    }
                case DebugStatRowType.LABEL_AND_VALUE:
                    {
                        var rowGo = new GameObject(stat.Label);
                        rowGo.transform.SetParent(this.transform, false);
                        rowGo.AddComponent<RectTransform>();
                        var horizontalLayoutGroup = rowGo.AddComponent<HorizontalLayoutGroup>();

                        // add label
                        var labelGo = new GameObject("label");
                        labelGo.transform.SetParent(rowGo.transform, false);
                        var labelText = CreateTextComponent(labelGo, stat.Label);

                        // add value
                        var valueGo = new GameObject("value");
                        valueGo.transform.SetParent(rowGo.transform, false);
                        var valueText = CreateTextComponent(valueGo, stat.GetValue());
                        valueText.alignment = TextAnchor.UpperRight;

                        stat.TextComponents.Add(labelText);
                        stat.TextComponents.Add(valueText);
                        break;
                    }
            }
        }

        var verticalLayoutGroup = this.AddComponent<VerticalLayoutGroup>();
        var contentSizeFitter = this.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private Text CreateTextComponent(GameObject go, string text = null)
    {
        var component = go.AddComponent<Text>();
        component.font = Font;
        component.text = text;
        return component;
    }
}
