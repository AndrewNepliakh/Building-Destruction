using System;
using System.Collections.Generic;
using System.Linq;
using Madkpv;
using UnityEngine;
using UnityEngine.Audio;

public class CarSoundController : MonoBehaviour
{
    [SerializeField] private AudioClipCrossFadeStruct[] _engineSoundClips;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    [SerializeField] private float _spatialBlend = 1;
    private AudioClipCrossFadeStruct _currentSoundClip => _engineSoundClips[_currentSoundClipIndex];
    private bool _isSoundEnabled = true;
    private float _previousEngineSpeed = 0;
    private int _currentSoundClipIndex = 0;
    private CrossFadePlayer _crossFadePlayer;

    private void Awake()
    {
        var audioSources = Enumerable.Range(0, 2).Select(x =>
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _mixerGroup;
            source.spatialBlend = _spatialBlend;
            return source;
        }).ToArray();
        _crossFadePlayer = new CrossFadePlayer(audioSources);
    }

    private void Start()
    {
        _engineSoundClips = _engineSoundClips.OrderBy(x => x.MinRpm).ToArray();
        _crossFadePlayer.PlayLoop(_currentSoundClip.AudioClip);
    }

    public void UpdateSound(float engineSpeed)
    {
        if (GetNextAudioClip(engineSpeed, out var nextSoundClip))
        {
            _crossFadePlayer.PlayLoop(nextSoundClip, .5f);
        }

        _previousEngineSpeed = engineSpeed;

        _crossFadePlayer.SetPitch(_currentSoundClip.GetPitch(engineSpeed));
    }

    private void Update()
    {
        _crossFadePlayer.Tick();
    }

    private bool GetNextAudioClip(float engineSpeed, out AudioClip nextClip)
    {
        if (engineSpeed < _previousEngineSpeed)
        {
            if (_currentSoundClipIndex > 0 && engineSpeed < _engineSoundClips[_currentSoundClipIndex].MinRpm)
            {
                _currentSoundClipIndex--;
                nextClip = _engineSoundClips[_currentSoundClipIndex].AudioClip;
                return true;
            }
        }
        else if (_currentSoundClipIndex < _engineSoundClips.Length - 1 &&
                 engineSpeed > _engineSoundClips[_currentSoundClipIndex].MaxRpm)
        {
            _currentSoundClipIndex++;
            nextClip = _engineSoundClips[_currentSoundClipIndex].AudioClip;
            return true;
        }

        nextClip = _currentSoundClip.AudioClip;
        return false;
    }

    public void EnableSound()
    {
        _isSoundEnabled = true;
    }

    public void DisableSound()
    {
        _isSoundEnabled = false;
        _crossFadePlayer.Stop();
    }

    public void OnDisable()
    {
        DisableSound();
    }

    [System.Serializable]
    private struct AudioClipCrossFadeStruct
    {
        public AudioClip AudioClip;
        public float MinRpm;
        public float MaxRpm;
        [SerializeField] private float MinPich;
        [SerializeField] private float MaxPitch;

        public float GetPitch(float engineSpeed)
        {
            var time = Mathf.InverseLerp(MinRpm, MaxRpm, engineSpeed);
            return Mathf.Lerp(MinPich, MaxPitch, time);
        }
    }

    private class CrossFadePlayer
    {
        private readonly AudioSource _audioSource1;
        private readonly AudioSource _audioSource2;

        private Queue<(AudioClip, float, float, bool)> _clipsQueue = new();
        private bool _crossfadeInProgress;
        private float _currentCrossFadeTimer;
        private float _currentCrossFadeTimerCounter;
        private bool _isDirectionForward = true;
        private float _currentMaxVolume;
        private float _currentTime;

        public CrossFadePlayer(AudioSource[] audioSources)
        {
            if (audioSources.Length < 2)
                throw new Exception("Tried to create CrossfadePlayer with less than 2 audio sources");
            _audioSource1 = audioSources[0];
            _audioSource2 = audioSources[1];
        }

        public void PlayLoop(AudioClip clip, float transitionDuration = 0, float maxVolume = 1) =>
            Play(clip, transitionDuration, true, maxVolume);

        public void PlayOneShot(AudioClip clip, float transitionDuration = 0, float maxVolume = 1) =>
            Play(clip, transitionDuration, false, maxVolume);

        private void Play(AudioClip clip, float crossFadeDuration, bool playLoop, float maxVolume)
        {
            _clipsQueue.Enqueue((clip, crossFadeDuration, maxVolume, playLoop));
            _crossfadeInProgress = true;
        }

        public void Play()
        {
            if (_isDirectionForward) _audioSource2.Play();
            else _audioSource1.Play();
        }

        public void Tick()
        {
            if (_crossfadeInProgress == false) return;

            if (_currentCrossFadeTimerCounter > 0)
            {
                _currentCrossFadeTimerCounter -= Time.deltaTime;
                _currentTime = Mathf.MoveTowards(_currentTime, 1, Time.deltaTime / _currentCrossFadeTimer);
                var volume = Mathf.Lerp(0, _currentMaxVolume, _currentTime);
                if (_isDirectionForward)
                {
                    _audioSource1.volume = 1 - volume;
                    _audioSource2.volume = volume;
                }
                else
                {
                    _audioSource1.volume = volume;
                    _audioSource2.volume = 1 - volume;
                }

                return;
            }

            if (_isDirectionForward)
            {
                _audioSource1.volume = 0;
                _audioSource2.volume = _currentMaxVolume;
                _audioSource1.Stop();
            }
            else
            {
                _audioSource1.volume = _currentMaxVolume;
                _audioSource2.volume = 0;
                _audioSource2.Stop();
            }


            if (_clipsQueue.Count == 0)
            {
                _crossfadeInProgress = false;
                return;
            }

            PrepareNextSound();
        }

        private void PrepareNextSound()
        {
            (var clip, var duration, var maxVolume, var loop) = _clipsQueue.Dequeue();

            _currentCrossFadeTimer = duration;
            _currentCrossFadeTimerCounter = duration;
            _currentMaxVolume = maxVolume;
            _crossfadeInProgress = true;
            _currentTime = 0;

            if (_audioSource1.isPlaying)
            {
                _audioSource2.loop = loop;
                _audioSource2.volume = 0;
                _audioSource2.clip = clip;
                _audioSource2.Play();
                _isDirectionForward = true;
            }
            else
            {
                _audioSource1.loop = loop;
                _audioSource1.volume = 0;
                _audioSource1.clip = clip;
                _audioSource1.Play();
                _isDirectionForward = false;
            }
        }

        public void Stop()
        {
            _audioSource1.Stop();
            _audioSource2.Stop();
        }

        public void Pause()
        {
            if (_isDirectionForward) _audioSource2.Pause();
            else _audioSource1.Pause();
        }

        public void Unpause()
        {
            if (_isDirectionForward) _audioSource2.UnPause();
            else _audioSource1.UnPause();
        }

        public void SetPitch(float value)
        {
            if (_isDirectionForward) _audioSource2.pitch = value;
            else _audioSource1.pitch = value;
        }
    }
}