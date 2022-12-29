using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RayFire;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEngine;
using Object = System.Object;

[CustomEditor(typeof(RayFireDestructionBuilder))]
public class RayFireDestructionBuilderEditor : Editor
{
    private const string _fbxPath = "Assets/FBX_DemolitionExports/";
    private bool _addRigidBodyToDestructedMeshes = true;
    private bool _makeConvexMeshCollider = true;
    private int _defaultFractureCount = 3;
    private float _defaultPiecesMass = 10;
    private SimType _defaultSimulationType;


    public override void OnInspectorGUI()
    {
        _addRigidBodyToDestructedMeshes = serializedObject.FindProperty("addRigidBodyToDestructedMeshes").boolValue;
        _makeConvexMeshCollider = serializedObject.FindProperty("makeConvexMeshCollider").boolValue;
        _defaultFractureCount = serializedObject.FindProperty("defaultFractureCount").intValue;
        _defaultPiecesMass = serializedObject.FindProperty("defaultPiecesMass").floatValue;
        _defaultSimulationType = (SimType)serializedObject.FindProperty("defaultSimulationType").enumValueIndex;

        _addRigidBodyToDestructedMeshes =
            GUILayout.Toggle(_addRigidBodyToDestructedMeshes, "Add rigidBody to Destructed Meshes");
        _makeConvexMeshCollider = GUILayout.Toggle(_makeConvexMeshCollider, "Make all mesh colliders convex");
        _defaultFractureCount = EditorGUILayout.IntField("Default shatter pieces count", _defaultFractureCount);
        _defaultPiecesMass = EditorGUILayout.FloatField("Default pieces mass", _defaultPiecesMass);
        _defaultSimulationType = (SimType)EditorGUILayout.EnumFlagsField("Default Sim Type", _defaultSimulationType);

        if (GUILayout.Button("Make Reference Demolition"))
        {
            MakeReferencesDemolition();
        }

        if (GUILayout.Button("Init Root"))
        {
            InitDestructionRoot();
        }

        if (GUILayout.Button("Flatten tree"))
        {
            FlattenTree();
        }

        serializedObject.FindProperty("addRigidBodyToDestructedMeshes").boolValue = _addRigidBodyToDestructedMeshes;
        serializedObject.FindProperty("makeConvexMeshCollider").boolValue = _makeConvexMeshCollider;
        serializedObject.FindProperty("defaultFractureCount").intValue = _defaultFractureCount;
        serializedObject.FindProperty("defaultPiecesMass").floatValue = _defaultPiecesMass;
        serializedObject.FindProperty("defaultSimulationType").enumValueIndex = (int)_defaultSimulationType;

        serializedObject.ApplyModifiedProperties();
    }

    private void FlattenTree()
    {
        var transform = ((RayFireDestructionBuilder)target).transform;
        RecursiveFlattener(transform, transform);
    }

    private void RecursiveFlattener(Transform lookIn, Transform topParent)
    {
        List<Transform> endNodes = new List<Transform>();
        List<Transform> delNodes = new List<Transform>();
        foreach (Transform child in lookIn)
        {
            Debug.Log("Child count: " + child.childCount, child);
            if (child.childCount > 0)
            {
                RecursiveFlattener(child, topParent);
                delNodes.Add(child);
                continue;
            }

            endNodes.Add(child);
        }

        endNodes.ForEach(x => x.parent = topParent);
        delNodes.ForEach(x => DestroyImmediate(x.gameObject));
    }

    private void InitDestructionRoot()
    {
        var transform = ((RayFireDestructionBuilder)target).transform;
        if (transform.TryGetComponent<RayfireConnectivity>(out var connectivity) == false)
        {
            connectivity = transform.AddComponent<RayfireConnectivity>();
        }

        if (transform.TryGetComponent<RayfireRigid>(out var rayfireRigid) == false)
        {
            rayfireRigid = transform.AddComponent<RayfireRigid>();
        }

        rayfireRigid.initialization = RayfireRigid.InitType.AtStart;
        rayfireRigid.activation = CreateActivation();
        rayfireRigid.objectType = ObjectType.MeshRoot;
        rayfireRigid.simulationType = _defaultSimulationType;

        connectivity.type = ConnectivityType.ByBoundingBox;
        connectivity.clusterize = true;
        connectivity.expand = 1;


        RFActivation CreateActivation()
        {
            return new RFActivation
            {
                byActivator = true,
                byConnectivity = true
            };
        }
    }

    private void MakeReferencesDemolition()
    {
        var transform = ((RayFireDestructionBuilder)target).transform;
        var renderers = transform.GetComponentsInChildren<MeshRenderer>();
        var exportBasePath = _fbxPath + $"/{target.name}/";
        if (Directory.Exists(exportBasePath) == false)
            Debug.Log(Directory.CreateDirectory(exportBasePath));

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy == false) DestroyImmediate(child.gameObject);
        }

        foreach (var meshRenderer in renderers)
        {
            if (meshRenderer.TryGetComponent<RayfireShatter>(out var shatter) == false)
            {
                shatter = meshRenderer.AddComponent<RayfireShatter>();
                shatter.voronoi.amount = _defaultFractureCount;
            }

            if (_makeConvexMeshCollider && meshRenderer.TryGetComponent<MeshCollider>(out var meshCollider))
                meshCollider.convex = true;

            shatter.DeleteFragmentsAll();
            shatter.Fragment();

            if (shatter.fragmentsAll.Count < 1) continue;

            var fragmentsParent = shatter.fragmentsLast[0].transform.parent;
            fragmentsParent.gameObject.SetActive(false);

            var rayfireRigid = InitRayfireRigid(meshRenderer);

            rayfireRigid.demolitionType = DemolitionType.ReferenceDemolition;

            var exportPath = exportBasePath + fragmentsParent.name + ".fbx";
            var exportedFbxPath = ModelExporter.ExportObject(exportPath, fragmentsParent);

            var asset = (ModelImporter)AssetImporter.GetAtPath(exportedFbxPath);
            asset.isReadable = true;
            asset.materialLocation = ModelImporterMaterialLocation.External;
            asset.materialSearch = ModelImporterMaterialSearch.Everywhere;
            AssetDatabase.ImportAsset(exportedFbxPath, ImportAssetOptions.ForceUpdate);

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(exportedFbxPath);
            var newGo = Instantiate(prefab, fragmentsParent.position, fragmentsParent.rotation,
                fragmentsParent.parent);

            if (_addRigidBodyToDestructedMeshes)
            {
                var parts = newGo.GetComponentsInChildren<MeshRenderer>();
                foreach (var part in parts)
                {
                    if (part.TryGetComponent<Rigidbody>(out var partRb) == false)
                        partRb = part.AddComponent<Rigidbody>();
                    partRb.mass = _defaultPiecesMass;
                    partRb.isKinematic = false;
                    partRb.useGravity = true;

                    if (part.TryGetComponent<MeshCollider>(out var collider) == false)
                        collider = part.AddComponent<MeshCollider>();
                    collider.convex = true;

                    // if (part.TryGetComponent<RayfireRigid>(out var partRb) == false)
                    //     partRb = part.AddComponent<RayfireRigid>();
                    // partRb.physics.massBy = MassType.RigidBodyComponent;
                    // partRb.physics.mass = 10;
                    // partRb.demolitionType = DemolitionType.None;
                    // partRb.simulationType = SimType.Dynamic;
                    // partRb.initialization = RayfireRigid.InitType.AtStart;
                }
            }

            newGo.SetActive(false);
            DestroyImmediate(fragmentsParent.gameObject);

            var refDemolition = new RFReferenceDemolition();
            refDemolition.reference = newGo;
            refDemolition.action = RFReferenceDemolition.ActionType.SetActive;
            refDemolition.addRigid = !_addRigidBodyToDestructedMeshes;
            rayfireRigid.referenceDemolition = refDemolition;
        }
    }

    private RayfireRigid InitRayfireRigid(MeshRenderer meshRenderer)
    {
        if (meshRenderer.TryGetComponent<RayfireRigid>(out var rayfireRigid) == true) return rayfireRigid;

        rayfireRigid = meshRenderer.AddComponent<RayfireRigid>();
        rayfireRigid.simulationType = _defaultSimulationType;
        rayfireRigid.initialization = RayfireRigid.InitType.AtStart;

        var damage = new RFDamage
        {
            collect = true,
            maxDamage = 120,
            enable = true
        };

        rayfireRigid.damage = damage;

        rayfireRigid.activation = new RFActivation
        {
            activatable = true, 
            byActivator = true,
            byConnectivity = true,
        };

        rayfireRigid.limitations = new RFLimitations
        {
            byCollision = true, 
            solidity = 10
        };
        
        return rayfireRigid;
    }
}