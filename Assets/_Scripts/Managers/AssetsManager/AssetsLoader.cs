using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace Managers
{
    public class AssetsLoader : IAssetsLoader
    {
        private GameObject _cachedObject;
        private AsyncOperationHandle<GameObject> _handle;

        public async Task<T> InstantiateAsset<T>(Transform parent = null)
        {
             _handle = Addressables.InstantiateAsync(typeof(T).ToString(), parent);
            _cachedObject = await _handle.Task;
            return TryGetComponent<T>();
        }

        public async Task<T> InstantiateAssetWithDI<T>(string ID, DiContainer diContainer, Transform parent = null)
        {
            _handle = Addressables.LoadAssetAsync<GameObject>(ID);
            var prefab = await _handle.Task;
            
            _cachedObject = diContainer.InstantiatePrefab(prefab, Vector3.zero, Quaternion.identity, parent);

            if (_cachedObject.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.one;
            }

            return TryGetComponent<T>();
        }

        private T TryGetComponent<T>()
        {
            if (_cachedObject.TryGetComponent(out T component) == false)
            {
                throw new NullReferenceException(
                    $"Object of type {typeof(T)} is null on attempt to load it from addressables");
            }

            return component;
        }

        public void UnloadAsset()
        {
            if (_cachedObject == null) return;

            _cachedObject.SetActive(false);
            Addressables.ReleaseInstance(_cachedObject);
            if (_cachedObject != null)
            {
                Object.Destroy(_cachedObject);
                Addressables.Release(_handle);
            }
            
            _cachedObject = null;
        }
    }
}