using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterpolateHelper
{
    public static float GetFactor(float sharpness, float deltaTime)
    {
        return 1f - Mathf.Exp(-sharpness * deltaTime);
    }

    public static bool HasChanged<T>(ref T oldValue, T newValue)
    {
        if (oldValue.Equals(newValue))
        {
            return false;
        }

        oldValue = newValue;
        return true;
    }

    public static T TrueIfChanged<T>(this T oldValue, T newValue, ref bool changed)
    {
        if (oldValue.Equals(newValue))
            return oldValue;

        changed = true;
        return newValue;
    }
}
