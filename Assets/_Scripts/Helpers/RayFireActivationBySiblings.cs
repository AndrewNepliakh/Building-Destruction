using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Madkpv;
using RayFire;
using Unity.VisualScripting;
using UnityEngine;

public class RayFireActivationBySiblings : MonoBehaviour
{
    private float _radius = 1;
    private int _maxRigidBodiesToCheck = 5;

    private List<Rigidbody> _rbs = new List<Rigidbody>();
    private RayfireRigid _rayfireRigid;
    private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
    private bool _isNotActive = true;
    [SerializeField] private float _pollTimeout = .1f;
    private Vector3 _center;

    private void Awake()
    {
        _rbs = GetComponentsInChildren<Rigidbody>().ToList();
    }

    private IEnumerator Start()
    {
        yield return null;
        _center = CenterOfExtents();
        var colliders = Physics.OverlapSphere(_center, _radius);
        foreach (var collider in colliders)
        {
            if (collider.attachedRigidbody == null || _rbs.Contains(collider.attachedRigidbody)) continue;
            _rigidbodies.Add(collider.attachedRigidbody);
            if (_rigidbodies.Count > _maxRigidBodiesToCheck) break;
        }

        do
        {
            foreach (var rigidbody in _rigidbodies)
            {
                if (rigidbody == null || rigidbody.isKinematic == false)
                {
                    OnSiblingActivation();
                    break;
                }
            }

            yield return Helpers.CachedDelay(_pollTimeout);
        } while (_isNotActive);
    }

    Vector3 CenterOfExtents()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer == null) return transform.position;
        var bounds = renderer.bounds;
        return bounds.center;
    }


    private void OnSiblingActivation()
    {
        _isNotActive = false;
       _rbs.ForEach(x => x.isKinematic = false);
    }

    private void OnDrawGizmosSelected() => Gizmos.DrawWireSphere(_center, _radius);
}