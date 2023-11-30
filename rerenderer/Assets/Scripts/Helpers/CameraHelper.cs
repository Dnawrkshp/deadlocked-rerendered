using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraHelper
{
    public static Vector3 WorldToViewportPointStretched(this Camera camera, Vector3 position)
    {
        var vs = camera.WorldToViewportPoint(position);
        //var stretch = Constants.POST_STRETCH;
        //vs.x = ((vs.x - 0.5f) * stretch.x) + 0.5f;
        //vs.y = ((vs.y - 0.5f) * stretch.y) + 0.5f;
        return vs;
    }
}
