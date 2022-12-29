using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Managers
{
    public abstract class PoolableByAssetsReferencesObjectManager<T> : PoolableObjectsManager<T> where T : class
    {
        public PoolableByAssetsReferencesObjectManager(DiContainer container) : base(container) { }

        protected AssetReference[] _assetReferences;

        public virtual void LoadResources(AssetReference[] assetReferences)
        {
            FlushAllPooledObjects();
            _assetReferences = assetReferences;
            LoadObjects();
        }

        protected void FlushAllPooledObjects()
        {
            if (_assetReferences == null || _assetReferences.Length < 1) return;

            foreach (var keyValuePair in _objectsPool)
            {
                keyValuePair.Value.Clear();
            }
            
            _objectsPool.Clear();
            _objectsPrefabs.Clear();
            
            foreach (var assetReference in _assetReferences)
            {
                assetReference.ReleaseAsset();
            }
        }

        protected override void LoadObjects()
        {
            LoadAssetsList();
        }

        protected virtual async void LoadAssetsList()
        {
            if (_assetReferences == null || _assetReferences.Length < 1)
            {
                Debug.LogWarning("Tried to load empty list of assetReferences");
                return;
            }

            List<Task> tasks = new List<Task>();

            foreach (var assetReference in _assetReferences)
            {
                var handle = assetReference.LoadAssetAsync<GameObject>();
                handle.Completed += InitPoolEntry;
                tasks.Add(handle.Task);
            }

            await Task.WhenAll(tasks);
            OnObjectsLoaded();
        }

        protected abstract void OnObjectsLoaded();

        protected abstract void InitPoolEntry(AsyncOperationHandle<GameObject> obj);
    }
}