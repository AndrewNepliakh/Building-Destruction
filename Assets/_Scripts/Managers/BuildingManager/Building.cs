using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RayFire;
using UnityEngine;

namespace Managers
{
    public class Building : MonoBehaviour, IBuilding
    {
        public AssetsLoader loader { get; set; }

        private IDemolishEstimator _estimator;
        private int _valueFor1Stars = 25;
        private int _valueFor2Stars = 40;
        private int _valueFor3Stars = 55;

        public void Init(object o)
        {
            _estimator = GetComponent<IDemolishEstimator>();
        }

        public int GetLevelResult()
        {
            var abs = _estimator.GetAbsoluteValue();
            var rem = _estimator.GetRemainedValue();
            var percentage = 100 - Mathf.CeilToInt((rem / abs) * 100.0f);

            var result = 0;
            if (percentage >= _valueFor1Stars) result = 1;
            if (percentage >= _valueFor2Stars) result = 2;
            if (percentage >= _valueFor3Stars) result = 3;

            return result;
        }
    }
}