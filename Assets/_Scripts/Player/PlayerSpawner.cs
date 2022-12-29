using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using Zenject;

public class PlayerSpawner : MonoBehaviour
{
    public event Action<ICarController> OnPlayerSpawned;


    [Inject] private GameSettingsSO _settings;
    [Inject] private DiContainer _container;
    [Inject] private ILevelManager _levelManager;
    private GameObject _instantiatedPlayer;
    private Vector3 _spawnPoint;


    private void Awake()
    {
        _levelManager.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnLevelLoaded(ILevel level)
    {
        _spawnPoint = level.SpawnPoint;
        Spawn();
    }

    private void Spawn()
    {
        if (_instantiatedPlayer != null) return;
        var handle = _settings.GetSelectedPlayerAsset().LoadAssetAsync();
        handle.Completed += (op) =>
        {
            _instantiatedPlayer = _container.InstantiatePrefab(op.Result);
            _instantiatedPlayer.transform.position = _spawnPoint;
            var carController = _instantiatedPlayer.GetComponent<ICarController>();
            OnPlayerSpawned?.Invoke(carController);
        };
    }

    private void OnDestroy()
    {
        _levelManager.OnLevelLoaded -= OnLevelLoaded;
        Destroy(_instantiatedPlayer);
        _settings.GetSelectedPlayerAsset().ReleaseAsset();
    }
}