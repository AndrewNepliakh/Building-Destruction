
using RayFire;
using UnityEngine;

[ExecuteInEditMode]
public class RayFireDestructionBuilder : MonoBehaviour
{
    public bool addRigidBodyToDestructedMeshes = true;
    public bool makeConvexMeshCollider = true;
    public int defaultFractureCount = 3;
    public float defaultPiecesMass = 10;
    public SimType defaultSimulationType; 
}