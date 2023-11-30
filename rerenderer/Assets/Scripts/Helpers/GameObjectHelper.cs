using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GameObjectHelper
{
    public static void SetHierarchyLayer(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
            SetHierarchyLayer(child.gameObject, layer);
    }
}
