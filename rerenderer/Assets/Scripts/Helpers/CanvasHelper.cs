using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasHelper
{
    public static Vector2 GetRatchetScreenSpaceToUnityScreenSpaceRatio(this Canvas canvas)
    {
        var ratio = (canvas.pixelRect.height / Constants.UI_SCREEN_HEIGHT) / canvas.scaleFactor;
        return new Vector2(ratio * (Constants.UI_SCREEN_WIDTH / Constants.SCREEN_WIDTH), ratio);
    }

    public static Vector2 RatchetTextSpaceToUnityScreenSpace(this Canvas canvas, float ratchetX, float ratchetY)
    {
        var scale = canvas.GetRatchetScreenSpaceToUnityScreenSpaceRatio();
        return new Vector2(ratchetX * scale.x, -ratchetY * scale.y);
    }

    public static Vector2 RatchetScreenSpaceToUnityScreenSpace(this Canvas canvas, float ratchetX, float ratchetY)
    {
        var scale = canvas.GetRatchetScreenSpaceToUnityScreenSpaceRatio();
        return new Vector2(ratchetX * scale.x, -ratchetY * scale.y);
    }

    public static Vector2 RatchetRelativeScreenSpaceToUnityScreenSpace(this Canvas canvas, float ratchetRelX, float ratchetRelY)
    {
        return RatchetScreenSpaceToUnityScreenSpace(canvas, ratchetRelX * Constants.SCREEN_WIDTH, ratchetRelY * Constants.UI_SCREEN_HEIGHT);
    }
}
