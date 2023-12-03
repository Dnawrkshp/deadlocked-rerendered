using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameMat
{
    struct Mat
    {
        public bool ZWrite;
        public RCHelper.GS_ZTEST ZTest;
        public Material Material;
    }

    private static List<Mat> _mats = new List<Mat>();

    public static Material GetUIMaterial(bool zwrite = false, RCHelper.GS_ZTEST ztest = RCHelper.GS_ZTEST.ZALWAYS)
    {
        var existing = _mats.FirstOrDefault(x => x.ZWrite == zwrite && x.ZTest == ztest);
        if (existing.Material)
            return existing.Material;

        var defaultUIMat = Resources.Load<Material>("Materials/UI/UISprite");
        if (!defaultUIMat) return null;

        // instantiate and configure material
        var materialInstance = Material.Instantiate(defaultUIMat);
        materialInstance.SetZTest(ztest);
        materialInstance.SetZWrite(zwrite);

        var mat = new Mat()
        {
            ZWrite = zwrite,
            ZTest = ztest,
            Material = materialInstance
        };
        _mats.Add(mat);

        return materialInstance;
    }

}
