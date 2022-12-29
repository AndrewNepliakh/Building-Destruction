using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

public abstract class PoolableObjectsManager<T> where T : class
{
    protected DiContainer _container;
    protected Dictionary<Type, IObjectPool<T>> _objectsPool = new Dictionary<Type, IObjectPool<T>>();
    protected Dictionary<Type, GameObject> _objectsPrefabs = new Dictionary<Type, GameObject>();
    protected Transform _objectsParent = null;

    public PoolableObjectsManager(DiContainer container)
    {
        _container = container;
    }

    public virtual W GetObject<W>() where W : MonoBehaviour
    {
        if (_objectsPool.TryGetValue(typeof(W), out var objectsPool) == false)
        {
            if (_objectsPrefabs.TryGetValue(typeof(W), out var prefab) == false)
            {
                return null;
            }

            // objectsPool = new ObjectPool<T>(() => OnCreate(objectsPool, prefab), OnGet, OnRelease, OnClearObj);
            objectsPool = MakeObjectPool(typeof(T), prefab);
            _objectsPool.Add(typeof(W), objectsPool); 
        }

        ObjectPool<T> MakeObjectPool(Type type, GameObject prefab)
        {
            Type poolType = typeof(ObjectPool<>);
            Type generic = poolType.MakeGenericType(type);
            object[] args =
            {
                new Func<T>(
                    () => OnCreate(objectsPool, prefab)),
                (Action<T>)OnGet,
                (Action<T>)OnRelease,
                (Action<T>)OnClearObj, false, 10, 100
            };
            return Activator.CreateInstance(generic, args) as ObjectPool<T>;
        }

        return objectsPool.Get() as W;
    }

    protected virtual void OnClearObj(T obj) { }

    protected virtual void OnRelease(T obj) { }

    protected virtual void OnGet(T obj) { }

    public virtual MonoBehaviour GetObjectByType(Type type)
    {
        Func<MonoBehaviour> method = GetObject<MonoBehaviour>;
        var generic = this.GetType().GetMethod(method.Method.Name).MakeGenericMethod(type);
        return generic?.Invoke(this, null) as MonoBehaviour;
    }

    protected virtual T OnCreate(IObjectPool<T> objectsPool, GameObject prefab)
    {
        var go = _container.InstantiatePrefab(prefab);
        go.transform.parent = _objectsParent;
        return go.GetComponent<T>();
    }

    public virtual void Initialize() => LoadObjects();

    /// <summary>
    /// LoadObjects must set _objectsPrefabs
    /// </summary>
    protected abstract void LoadObjects();
}