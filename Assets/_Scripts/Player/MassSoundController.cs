using System.Collections;
using System.Collections.Generic;
using Madkpv;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class MassSoundController : MonoBehaviour
{
    [SerializeField] private float _powerThreshold = .5f;
    [SerializeField] private float _minTimeBetweenSounds = .1f;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    [SerializeField,Range(0,1)] private float _spatialBlend = 1;

    private List<CollisionSoundSource> _sources;
    IObjectPool<AudioSource> _audioSourcesPool;
    private Transform _sourcesParent;
    private float _lastTimeSoundPlayed;

    private void Awake()
    {
        _sources = new List<CollisionSoundSource>(GetComponentsInChildren<CollisionSoundSource>(true));
        _audioSourcesPool =
            new ObjectPool<AudioSource>(OnAudioSourceCreate, OnAudioSourceGet, OnAudioSourceRelease, null, true, 10);
        _sourcesParent = new GameObject("[AudioSourcesPool]").transform;
        _sourcesParent.parent = transform;
    }

    private void OnAudioSourceRelease(AudioSource source)
    {
        source.gameObject.SetActive(false);
    }

    private void OnAudioSourceGet(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(true);
    }

    private AudioSource OnAudioSourceCreate()
    {
        AudioSource source = new GameObject("AudioSource").AddComponent<AudioSource>();
        source.transform.parent = _sourcesParent;
        source.loop = false;
        source.playOnAwake = false;
        source.spatialBlend = _spatialBlend;
        source.outputAudioMixerGroup = _mixerGroup;
        return source;
    }
    

    private void Start()
    {
        foreach (var source in _sources)
        {
            source.OnCollisionSoundRequested += OnSoundRequested;
        }
    }
    
    

    private void OnSoundRequested(float power, AudioClip clip, Transform sourceTransform)
    {
        if (power < _powerThreshold || Time.time - _lastTimeSoundPlayed < _minTimeBetweenSounds) return;
        // Debug.Log($"Played with power: {power}");
        _lastTimeSoundPlayed = Time.time;
        
        var audioSource = _audioSourcesPool.Get();
        audioSource.PlayOneShot(clip,power);
        StartCoroutine(AudioSourceReleaseRoutine(clip.length, audioSource));
    }

    IEnumerator AudioSourceReleaseRoutine(float realeaseAfter, AudioSource audioSource)
    {
        yield return new WaitForSeconds(realeaseAfter);
        _audioSourcesPool.Release(audioSource);
    }
    
    private void OnDestroy()
    {
        foreach (var source in _sources)
        {
            source.OnCollisionSoundRequested -= OnSoundRequested;
        }
    }
}