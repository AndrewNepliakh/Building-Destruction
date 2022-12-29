using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelPositionController : MonoBehaviour
{
    [SerializeField] private WheelCollider _referenceCollider;

    private void Update()
    {
        _referenceCollider.GetWorldPose(out var pos,out var rotation);
        transform.position = pos;
        transform.rotation = rotation;
    }
}
