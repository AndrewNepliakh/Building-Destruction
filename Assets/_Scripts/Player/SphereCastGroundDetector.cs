using UnityEngine;

public class SphereCastGroundDetector : MonoBehaviour, IGroundDetector
{
    [SerializeField] private Transform _groundProbeTransform;
    [SerializeField] private float _probeRadius;
    [SerializeField] private LayerMask _groundLayerMask;
    private Collider[] _groundColliders = new Collider[1];

    public bool IsGrounded() =>
        Physics.OverlapSphereNonAlloc(_groundProbeTransform.position, _probeRadius, _groundColliders,
            _groundLayerMask) > 0;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundProbeTransform.position, _probeRadius);
    }
}