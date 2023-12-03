using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GameTex
{
    private static Dictionary<int, Texture2D> cache = new Dictionary<int, Texture2D>();
    private static int? cacheLevelId = null;

    public static Texture2D GetEffectTex(int id)
    {
        CheckCache();

        if (cache.TryGetValue(id, out Texture2D sprite) && sprite)
            return sprite;

        return cache[id] = Load(id);
    }

    private static void CheckCache()
    {
        var level = RCHelper.GetCurrentLevel();
        if (level == null)
        {
            cache.Clear();
            cacheLevelId = null;
            return;
        }

        if (level != cacheLevelId)
        {
            cache.Clear();
            cacheLevelId = level;
        }
    }

    private static Texture2D Load(int id)
    {
        if (string.IsNullOrEmpty(Config.Singleton.GameDataPath)) return null;
        if (cacheLevelId == null) return null;

        var spritePath = Path.Combine(Config.Singleton.GameDataPath, "levels", cacheLevelId.Value.ToString(), "base", "fx", $"{id}.png");
        if (!File.Exists(spritePath)) return null;

        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false, true);
        var data = File.ReadAllBytes(spritePath);
        tex.LoadImage(data);
        tex.alphaIsTransparency = true;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        return tex;
    }

}
