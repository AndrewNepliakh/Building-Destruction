using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CarSlotSpawner : MonoBehaviour
{
    private AssetReferenceGameObject _carAsset;
    [SerializeField] private float _visibilityCheckTimeout = .1f;
    [SerializeField] private float _visibilityThreshold = .8f;
    private Transform _cameraTransform;

    public void Init(AssetReferenceGameObject asset, Transform cameraTransform, List<ColorController> carColors,
        Action<int> callback, int index)
    {
        _cameraTransform = cameraTransform;
        _carAsset = asset;
        StartCoroutine(VisibilityDetectionRoutine(carColors, callback, index));
    }

    IEnumerator VisibilityDetectionRoutine(List<ColorController> carColors, Action<int> callback, int index)
    {
        while (VisibilityCheck() == false)
            yield return new WaitForSeconds(_visibilityCheckTimeout);

        Spawn(carColors, callback, index);
    }

    private void Spawn(List<ColorController> carColors, Action<int> callback, int index)
    {
        _carAsset.InstantiateAsync(transform.position, transform.rotation, transform).Completed += Foo;

        void Foo(AsyncOperationHandle<GameObject> op)
        {
            var result = op.Result;
            var colorController = result.GetComponent<ColorController>();
            carColors.Add(colorController);
            callback?.Invoke(index);
        }
    }

    bool VisibilityCheck() =>
        Vector3.Dot((transform.position - _cameraTransform.position).normalized, _cameraTransform.forward) >
        _visibilityThreshold;
}