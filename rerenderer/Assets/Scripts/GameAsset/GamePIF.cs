using RC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public static class GamePIF
{
    class PIFCache
    {
        public Texture2D Texture;
        public float TimeLastRequested;
        public byte[] WeakHash;
    }

    private static Dictionary<RCPointer, PIFCache> cache = new Dictionary<RCPointer, PIFCache>();
    private static SHA256 hasher = SHA256.Create();

    public static Texture2D GetPIFTex(RCPointer pImage)
    {
        if (!pImage.HasValue()) return null;

        var hash = GetHash(pImage);
        if (hash == null) return null;

        if (cache.TryGetValue(pImage, out var entry) && entry != null && entry.Texture && entry.WeakHash != null && entry.WeakHash.SequenceEqual(hash))
        {
            entry.TimeLastRequested = Time.fixedUnscaledTime;
            return entry.Texture;
        }

        var tex = Load(pImage);
        if (!tex)
        {
            cache.Remove(pImage);
            return null;
        }

        if (entry == null) entry = new PIFCache();
        entry.Texture = tex;
        entry.TimeLastRequested = Time.fixedUnscaledTime;
        entry.WeakHash = hash;
        cache[pImage] = entry;
        return tex;
    }

    private static byte[] GetHash(RCPointer pImage)
    {
        if (!pImage.HasValue()) return null;

        var magic = EmuInterop.ReadInt32(pImage.Address);
        if (magic != 0x50494632) return null;

        // gets the header and the palette
        // for 16 color palette we'll also get some of the texture
        var bytes = EmuInterop.ReadBytes(pImage.Address, 0x20 + 0x400);

        // we don't want to parse each pif every fixedupdate
        // but unfortunately the game can at any point load a new texture into the same block of memory
        // so we hash just the header and palette in the hopes that we can detect when 
        return hasher.ComputeHash(bytes.Value.Buffer, (int)bytes.Value.Offset, (int)bytes.Value.Length);
    }

    private static Texture2D Load(RCPointer pImage)
    {
        if (!pImage.HasValue()) return null;

        var magic = EmuInterop.ReadInt32(pImage.Address);
        if (magic != 0x50494632) return null;

        var width = EmuInterop.ReadInt32(pImage.Address + 0x08);
        var height = EmuInterop.ReadInt32(pImage.Address + 0x0c);
        var format = EmuInterop.ReadInt32(pImage.Address + 0x10);
        if (!width.HasValue || !height.HasValue || !format.HasValue) return null;

        ByteSlice? palette;
        ByteSlice? indices;
        switch (format)
        {
            case 0x13:
                {
                    palette = EmuInterop.ReadBytes(pImage.Address + 0x20, 0x400);
                    indices = EmuInterop.ReadBytes(pImage.Address + 0x420, width.Value * height.Value);
                    break;
                }
            case 0x93:
                {
                    palette = EmuInterop.ReadBytes(pImage.Address + 0x20, 0x400);
                    indices = EmuInterop.ReadBytes(pImage.Address + 0x420, width.Value * height.Value);
                    break;
                }
            case 0x94:
                {
                    palette = EmuInterop.ReadBytes(pImage.Address + 0x20, 0x40);
                    indices = EmuInterop.ReadBytes(pImage.Address + 0x60, (width.Value * height.Value) / 2);
                    break;
                }
            default: throw new NotImplementedException();
        }

        // parse
        byte[] pixelData = new byte[width.Value * height.Value * 4];
        for (int i = 0; i < indices.Value.Length; ++i)
        {
            switch (format)
            {
                case 0x13:
                    {
                        var index = RCHelper.DecodePaletteIndex(indices?[i] ?? 0);
                        var map = RCHelper.RemapPixelIndexFlipY(i, width.Value, height.Value);
                        pixelData[(map * 4) + 0] = palette?[(index * 4) + 0] ?? 0;
                        pixelData[(map * 4) + 1] = palette?[(index * 4) + 1] ?? 0;
                        pixelData[(map * 4) + 2] = palette?[(index * 4) + 2] ?? 0;
                        pixelData[(map * 4) + 3] = palette?[(index * 4) + 3] ?? 0;
                        break;
                    }
                case 0x93:
                    {
                        var map = RCHelper.RemapPixelIndexRC4(i, width.Value);
                        var index = RCHelper.DecodePaletteIndex(indices?[map] ?? 0);
                        pixelData[(map * 4) + 0] = palette?[(index * 4) + 0] ?? 0;
                        pixelData[(map * 4) + 1] = palette?[(index * 4) + 1] ?? 0;
                        pixelData[(map * 4) + 2] = palette?[(index * 4) + 2] ?? 0;
                        pixelData[(map * 4) + 3] = palette?[(index * 4) + 3] ?? 0;
                        break;
                    }
                case 0x94:
                    {
                        var index1 = indices.Value[i] & 0x0f;
                        var index2 = indices.Value[i] >> 4;

                        var map = RCHelper.RemapPixelIndexRC4((i*2) + 0, width.Value);
                        map = RCHelper.RemapPixelIndexFlipY(map, width.Value, height.Value);
                        pixelData[(map * 4) + 0] = palette?[(index1 * 4) + 0] ?? 0;
                        pixelData[(map * 4) + 1] = palette?[(index1 * 4) + 1] ?? 0;
                        pixelData[(map * 4) + 2] = palette?[(index1 * 4) + 2] ?? 0;
                        pixelData[(map * 4) + 3] = palette?[(index1 * 4) + 3] ?? 0;

                        map = RCHelper.RemapPixelIndexRC4((i*2) + 1, width.Value);
                        map = RCHelper.RemapPixelIndexFlipY(map, width.Value, height.Value);
                        pixelData[(map * 4) + 0] = palette?[(index2 * 4) + 0] ?? 0;
                        pixelData[(map * 4) + 1] = palette?[(index2 * 4) + 1] ?? 0;
                        pixelData[(map * 4) + 2] = palette?[(index2 * 4) + 2] ?? 0;
                        pixelData[(map * 4) + 3] = palette?[(index2 * 4) + 3] ?? 0;
                        break;
                    }
            }
        }

        // load into Texture2D
        var tex = new Texture2D(width.Value, height.Value, TextureFormat.RGBA32, false, false);
        tex.alphaIsTransparency = true;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixelData(pixelData, 0);
        tex.Apply();

        return tex;
    }
}
