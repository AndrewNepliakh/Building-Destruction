using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public abstract class PoolableAddressablesObjectsManager<T> : PoolableObjectsManager<T> where T : class
{
    /// <summary>
    /// addressableKey must contain Label or single asset key
    /// </summary>
    /// <param name="container"></param>
    /// <param name="addressablesKey"></param>
    /// <returns></returns>
    public PoolableAddressablesObjectsManager(DiContainer container, string addressablesKey) : base(container)
    {
        _addressablesKey = addressablesKey;
    }

    protected PoolableAddressablesObjectsManager(DiContainer container) : base(container) { }

    protected string _addressablesKey;
    protected override void LoadObjects()
    {
        Addressables.LoadAssetsAsync<GameObject>(_addressablesKey, obj =>
        {
            var component = obj.GetComponent<T>();
            var type = component.GetType();
            _objectsPrefabs.Add(type, obj);
        }).Completed += OnObjectsLoaded;
    }

    protected virtual void OnObjectsLoaded(AsyncOperationHandle<IList<GameObject>> asyncOperationHandle) { }
}