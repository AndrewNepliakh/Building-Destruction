using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DetachablePart : MonoBehaviour, IResetable
{
    [SerializeField] protected float _ejectForceMultiplier = 100;
    protected Rigidbody _rb;
    protected Vector3 _startPosition;
    protected Quaternion _startRotation;
    protected Transform _parent;

    protected float _jointBreakForce;
    protected Rigidbody _jointConnectedBody;
    protected bool _isJointBroken = false;
    private FixedJoint _joint;


    protected virtual void Awake()
    {
        _joint = GetComponent<FixedJoint>();
        _rb = GetComponent<Rigidbody>();
        _jointBreakForce = _joint.breakForce;
        _jointConnectedBody = _joint.connectedBody;
    }

    protected virtual void Start()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;
        _parent = transform.parent;
    }

    public virtual void OnResetPlayer()
    {
        if (_isJointBroken == false) return;
        _isJointBroken = false;
        if (TryGetComponent<FixedJoint>(out var fixedJoint)) return;
        transform.parent = _parent;
        transform.localPosition = _startPosition;
        transform.localRotation = _startRotation;
        ResetForces();
        var joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = _jointConnectedBody;
        joint.breakForce = _jointBreakForce;
    }

    private void ResetForces() => _rb.velocity = _rb.angularVelocity = Vector3.zero;

    public virtual void MakeJointUnbreakable()
    {
        if(_isJointBroken || _joint == null) return;
        _joint.breakForce = float.PositiveInfinity;
    }

    public virtual void MakeJointBreakable()
    {
        if (_isJointBroken || _joint == null) return;
        ResetForces();
        _joint.breakForce = _jointBreakForce;

    }

    protected virtual void OnJointBreak(float breakForce)
    {
        transform.parent = null;
        _isJointBroken = true;
        _rb.AddForce(transform.up * _ejectForceMultiplier, ForceMode.Impulse);
    }
}