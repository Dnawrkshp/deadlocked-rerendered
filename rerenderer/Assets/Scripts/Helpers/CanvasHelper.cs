using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CanvasHelper
{
    public static Vector2 RatchetScreenSpaceToUnityPixelSpace(this Canvas canvas, float x, float y)
    {
        var bh = canvas.pixelRect.height / canvas.scaleFactor;
        var bw = bh * (Constants.SCREEN_WIDTH / Constants.SCREEN_HEIGHT);

        return new Vector2(x * bw, -y * bh);
    }

    public static void SetClippingRectFromSwizzle(this MaskableGraphic graphic, Vector4 scissor)
    {
        var bh = graphic.canvas.pixelRect.height / graphic.canvas.scaleFactor;
        var bw = bh * (Constants.SCREEN_WIDTH / Constants.SCREEN_HEIGHT);

        var width = scissor.y - scissor.x;
        var height = scissor.w - scissor.z;
        var x = scissor.x * bw;
        var y = (1 - scissor.w) * bh;

        var r = new Rect();
        r.x = (-bw * 0.5f) + x;
        r.y = (-bh * 0.5f) + y;
        r.width = bw * width;
        r.height = bh * height;

        graphic.canvasRenderer.EnableRectClipping(r);
    }
}
