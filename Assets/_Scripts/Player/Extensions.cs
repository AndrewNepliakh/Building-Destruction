using System.Collections;
using Madkpv;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions
{
    public static Vector3 ClampX(this Vector3 position, float value) =>
        new(Mathf.Clamp(position.x, -value, value), position.y, position.z);
    
}
