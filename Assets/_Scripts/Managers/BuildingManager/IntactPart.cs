using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class IntactPart : MonoBehaviour, ISwitchablePart
    {
        public IntactPartType Type;

        [SerializeField] private DestroyableElement _destroyableElement;
        [SerializeField] private List<IntactPart> _intactParts;
        [SerializeField] private float _XrayPosOffset = 0.05f;
        [SerializeField] private float _ZrayPosOffset = 0.05f;

        private float _length = 0.5f;
        private float _impactVelocityValue = 12.0f;

        public void Start()
        {
            if (Type == IntactPartType.Default)
                GetRelativeParts();
        }

        private void GetRelativeParts()
        {
            var collider = GetComponent<BoxCollider>();

            var xOffset = collider.bounds.size.x * _XrayPosOffset;
            var zOffset = collider.bounds.size.z * _ZrayPosOffset;

            Vector3[] positions =
            {
                new(-xOffset, 0.0f, -zOffset),
                new(-collider.bounds.size.x + xOffset, 0.0f, -zOffset),
                new(-xOffset, 0.0f, -collider.bounds.size.z + zOffset),
                new(-collider.bounds.size.x + xOffset, 0.0f, -collider.bounds.size.z + zOffset),
                new(-collider.bounds.size.x / 2.0f, 0.0f, -collider.bounds.size.z / 2.0f)
            };

            _intactParts = new List<IntactPart>(positions.Length);

            for (var i = 0; i < positions.Length; i++)
            {
                var rayStart = collider.bounds.max + positions[i];

                if (i < 4)
                {
                    Debug.DrawRay(rayStart, Vector3.up * _length, Color.red, 10.0f);

                    if (Physics.Raycast(new Ray(rayStart, Vector3.up * _length), out var hit, _length))
                    {
                        if (hit.collider.TryGetComponent<IntactPart>(out var intactPart))
                        {
                            if (!_intactParts.Contains(intactPart))
                            {
                                _intactParts.Add(intactPart);
                            }
                        }
                    }
                }
                else
                {
                    Debug.DrawRay(rayStart, Vector3.up * _length, Color.green, 10.0f);

                    if (Physics.Raycast(new Ray(rayStart, Vector3.up * _length), out var hit, _length,
                            ~ LayerMask.GetMask("Ground")))
                    {
                        if (hit.collider.TryGetComponent<IntactPart>(out var intactPart))
                        {
                            if (!_intactParts.Contains(intactPart))
                            {
                                _intactParts.Add(intactPart);
                            }
                        }
                    }
                }
            }
        }

        public void Init(DestroyableElement destroyableElement)
        {
            _destroyableElement = destroyableElement;
        }

        public void SwitchOff()
        {
            foreach (var intactPart in _intactParts)
            {
                intactPart.SwitchOff();
            }

            _destroyableElement.Prepare();
        }
    }

    public enum IntactPartType
    {
        Default,
        Ceiling
    }
}