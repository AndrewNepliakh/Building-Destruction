using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class BrokenPart : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> _rigidbodies;
        public void Init()
        {
            _rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
        }
    }
}