using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RadarHelper
{
    public static Vector2 WorldSpacePositionToRadarSpacePosition(Transform referencePoint, Vector3 worldSpacePosition)
    {
        var dt = worldSpacePosition - referencePoint.position;
        return new Vector2(dt.x / referencePoint.localScale.x, dt.z / referencePoint.localScale.z);
    }

    public static Vector3 RadarSpacePositionToWorldSpacePosition(Transform referencePoint, Vector2 radarSpacePosition)
    {
        return referencePoint.position + new Vector3(radarSpacePosition.x * referencePoint.localScale.x, 0, radarSpacePosition.y * referencePoint.localScale.z);
    }

    public static float WorldSpaceRotationToRadarSpaceYawDegrees(Transform referencePoint, Quaternion worldSpaceRotation)
    {
        var f1 = worldSpaceRotation * Vector3.forward;
        var f2 = referencePoint.rotation * Vector3.forward;

        return Mathf.Rad2Deg * (Mathf.Atan2(f1.z, f1.x) - Mathf.Atan2(f2.z, f2.x));
    }
}
