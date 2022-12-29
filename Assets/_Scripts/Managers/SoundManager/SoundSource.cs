using Managers;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class SoundSource : MonoBehaviour, ISoundSource
{
   [SerializeField] protected AudioSource _audioSource;

    public AssetsLoader AssetsLoader { get; private set; }

    public void Init(AssetsLoader assetsLoader)
    {
        AssetsLoader = assetsLoader;
    }

    public void Play()
    {
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }
}