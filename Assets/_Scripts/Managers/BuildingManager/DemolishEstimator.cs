using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Managers
{
    public class DemolishEstimator : MonoBehaviour, IDemolishEstimator
    {
        [SerializeField] private List<DestroyableElement> _destroyableElements;

        public float GetRemainedValue()
        {
            var destroyed = _destroyableElements.Count(destroyable => destroyable.IsDestroyed);
            var abs = GetAbsoluteValue();
            return abs - destroyed;
        }

        public float GetAbsoluteValue() => _destroyableElements.Count;
        
        [ContextMenu("Construct")]
        private void Construct()
        {
            _destroyableElements.Clear();
            _destroyableElements = transform.GetComponentsInChildren<DestroyableElement>().ToList();
        }
    }
}