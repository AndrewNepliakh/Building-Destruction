using System;
using RayFire;
using UnityEngine;

namespace Managers
{
    public class RayFireDemolishEstimator : MonoBehaviour, IDemolishEstimator
    {
        private RayfireConnectivity _rayfireConnectivity;

        private void Awake()
        {
            _rayfireConnectivity = GetComponentInChildren<RayfireConnectivity>();
        }
        
        public float GetRemainedValue() => _rayfireConnectivity.AmountIntegrity;
        
        public float GetAbsoluteValue() => 100.0f;
    }
}