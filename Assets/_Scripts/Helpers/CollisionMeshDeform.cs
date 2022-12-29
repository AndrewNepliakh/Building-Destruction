using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionMeshDeform : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;

    private Vector3[] _vertices;
    private Mesh _mesh;
    private Vector3[] _normals;
    private Vector3[] _originVertices;
    [SerializeField] private float _affectDistance = 1;
    [SerializeField] private float _forceInfluence = 1;
    [SerializeField] private float _maxForceEffect = .2f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _minTimeBetweenCollisions = .5f;
    private float _previousCollisionTime = 0;

    void Start()
    {
        _mesh = _meshFilter.sharedMesh;
        _originVertices = _mesh.vertices;
        _vertices = _mesh.vertices;
        _normals = _mesh.normals;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((Time.time - _previousCollisionTime) < _minTimeBetweenCollisions) return;
        if (((1 << collision.gameObject.layer) & _layerMask) == 0) return;

        _previousCollisionTime = Time.time;
        var contact = collision.contacts[0];
        var point = transform.InverseTransformPoint(contact.point);
        var impactDir = contact.normal;
        var forceMagnitude = collision.impulse.magnitude;

        for (var i = 0; i < _vertices.Length; i++)
        {
            var vertex = (_vertices[i]);
            var dir = vertex - point;
            var distance = dir.magnitude;

            if (dir.magnitude > _affectDistance) continue;

            var impactForceInfluence = Mathf.Clamp(_forceInfluence * forceMagnitude, 0, _maxForceEffect);
            var distanceInfluence = Mathf.Clamp01(1 - distance / _affectDistance);

            var newVertex =
                vertex + impactForceInfluence * distanceInfluence * impactDir;

            _vertices[i] = newVertex;
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateNormals();
    }

    private void OnDestroy() => ResetDeform();

    public void ResetDeform()
    {
        _mesh.vertices = _originVertices;
        _vertices = _mesh.vertices;
        _mesh.RecalculateNormals();
    }
}