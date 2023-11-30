using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{
    public static Vector3 GetScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    public static Vector3 GetScaleSigned(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = Mathf.Sign(matrix.lossyScale.x) * new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = Mathf.Sign(matrix.lossyScale.y) * new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = Mathf.Sign(matrix.lossyScale.z) * new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    public static Vector3 GetRandomDirection()
    {
        var theta = Random.Range(-Mathf.PI, Mathf.PI);
        var phi = Random.Range(-Mathf.PI, Mathf.PI);

        var sinTheta = Mathf.Sin(theta);
        var sinPhi = Mathf.Sin(phi);
        var cosPhi = Mathf.Cos(phi);

        return new Vector3(sinTheta * cosPhi, sinTheta * sinPhi, Mathf.Cos(theta));
    }

    public static float Deadzone(float value, float max, float min, float deadzone)
    {
        if (value < deadzone && value >= 0) return 0;
        if (value > -deadzone && value <= 0) return 0;
        return (value - (Mathf.Sign(value) * deadzone)) / ((max - min) - deadzone);
    }
}
