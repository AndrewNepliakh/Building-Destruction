using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class DetachableHingeJoint : DetachablePart
{
    private JointLimits _startJointLimits;
    private Vector3 _startJoinAxis;
    private Vector3 _startAnchor;
    private bool _startUseLimits;
    private HingeJoint _joint;

    protected override void Awake()
    {
        _joint = GetComponent<HingeJoint>();
        _rb = GetComponent<Rigidbody>();
        _jointBreakForce = _joint.breakForce;
        _jointConnectedBody = _joint.connectedBody;
        _startJointLimits = _joint.limits;
        _startJoinAxis = _joint.axis;
        _startAnchor = _joint.anchor;
        _startUseLimits = _joint.useLimits;
    }

    public override void OnResetPlayer()
    {
        if (_isJointBroken == false) return;
        _isJointBroken = false;
        transform.parent = _parent;
        transform.localPosition = _startPosition;
        transform.localRotation = _startRotation;
        _rb.velocity = _rb.angularVelocity = Vector3.zero;
        var joint = gameObject.AddComponent<HingeJoint>();
        joint.connectedBody = _jointConnectedBody;
        joint.breakForce = float.PositiveInfinity;
        joint.limits = _startJointLimits;
        joint.useLimits = _startUseLimits;
        joint.axis = _startJoinAxis;
        joint.anchor = _startAnchor;
        _joint = joint;
    }


    public override void MakeJointUnbreakable()
    {
        if (_isJointBroken) return;
        _joint.breakForce = float.PositiveInfinity;
    }

    public override void MakeJointBreakable()
    {
        if (_isJointBroken) return;
        _joint.breakForce = _jointBreakForce;
    }
}