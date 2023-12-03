using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialHelper
{
    public static void SetZWrite(this Material mat, bool zwrite)
    {
        mat.SetInt("_ZWrite", zwrite ? 1 : 0);
    }

    public static void SetZTest(this Material mat, UnityEngine.Rendering.CompareFunction ztest)
    {
        mat.SetInt("_ZTest", (int)ztest);
    }

    public static void SetZTest(this Material mat, RCHelper.GS_ZTEST ztest)
    {
        switch (ztest)
        {
            case RCHelper.GS_ZTEST.ZNOUSE: mat.SetZTest(UnityEngine.Rendering.CompareFunction.Disabled); break;
            case RCHelper.GS_ZTEST.ZALWAYS: mat.SetZTest(UnityEngine.Rendering.CompareFunction.Always); break;
            case RCHelper.GS_ZTEST.ZGREATER: mat.SetZTest(UnityEngine.Rendering.CompareFunction.Less); break;
            case RCHelper.GS_ZTEST.ZGEQUAL: mat.SetZTest(UnityEngine.Rendering.CompareFunction.LessEqual); break;
        }
    }
}
