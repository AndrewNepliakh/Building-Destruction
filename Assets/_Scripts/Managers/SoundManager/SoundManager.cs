using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using IInitializable = Zenject.IInitializable;

public class SoundManager : ISoundManager, IInitializable
{
    [Inject] private DiContainer _container;
    private Transform _soundSourcesParent;
    private SoundSource _currentSoundSource;
    
    private Dictionary<Type, SoundSource> _soundSources = new();

    public void Initialize()
    {
        try
        {
            _soundSourcesParent = GameObject.Find("[AudioSources]").GetComponent<Transform>();
        }
        catch
        {
            var parentGo = _container.CreateEmptyGameObject("[AudioSources]");
            _soundSourcesParent = parentGo.transform;
        }
    }

    public async void PlayAudioSource<T>() where T : SoundSource
    {
        if (_currentSoundSource != null && _currentSoundSource is T)
        {
            _currentSoundSource.Play();
        }
        else
        {
            if (_soundSources.TryGetValue(typeof(T), out var source))
            {
                _currentSoundSource = source;
                _currentSoundSource.Play();
            }
            else
            {
                var loader = new AssetsLoader();
                _currentSoundSource = await loader.InstantiateAsset<T>(_soundSourcesParent);
                _soundSources.Add(typeof(T), _currentSoundSource);
                _currentSoundSource.Init(loader);
                _currentSoundSource.Play();
            }
        }
    }

    public void Clear()
    {
        foreach (var soundSource in _soundSources.Values)
        {
            soundSource.AssetsLoader.UnloadAsset();
        }

        _soundSources.Clear();
    }
}