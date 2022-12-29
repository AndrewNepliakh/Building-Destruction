using System;
using Madkpv;
using UnityEngine;
using Random = UnityEngine.Random;

public class CollisionSoundSource : MonoBehaviour
{
    public event Action<float, AudioClip, Transform> OnCollisionSoundRequested;
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private bool _enablePlayItSelf;

    [SerializeField] private float _maxSpeedMagnitude = 100;

    [SerializeField, Range(0, 1)] private float _angleFactorThreshold = .9f;
    [SerializeField, Range(0, 1)] private float _forceInfluence = 1f;
    [SerializeField, Range(0, 1)] private float _playThreshold = .2f;
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_layerMask.CheckLayerMask(collision.gameObject.layer) == false) return;

        var overallFactor = OverallFactor(collision);


        if (_enablePlayItSelf && overallFactor > _playThreshold && _audioSource != null)
        {
            _audioSource.PlayOneShot(GetRandomClip(), overallFactor);
        }
        else
        {
            OnCollisionSoundRequested?.Invoke(overallFactor, GetRandomClip(), transform);
        }
    }

    private AudioClip GetRandomClip() => _audioClips[Random.Range(0, _audioClips.Length)];

    private float OverallFactor(Collision collision)
    {
        var contact = collision.contacts[0];
        var speedMagnitude = collision.relativeVelocity.magnitude;
        var forceFactor = Mathf.Clamp(speedMagnitude, 0, _maxSpeedMagnitude) / _maxSpeedMagnitude;
        var angleFactor =
            Mathf.Clamp01(Vector3.Dot(contact.normal, collision.relativeVelocity.normalized));

        angleFactor = angleFactor > _angleFactorThreshold ? angleFactor : 0;

        var overallFactor = forceFactor * _forceInfluence * angleFactor;
        return overallFactor;
    }
}