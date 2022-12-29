using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RemoveOnSlowDevice : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        var isDeviceSlow = PlayerPrefs.GetInt(Constants.FAST_DEVICE_PLAYERPREFS_KEY, 1) < 1;
        if (isDeviceSlow == false) return;
        if (Random.value > .5f) return;
        Destroy(gameObject,Random.Range(1f,3f));
        // _meshRenderer = GetComponent<MeshRenderer>();
        // StartCoroutine(HideObjectRoutine());
    }

    IEnumerator HideObjectRoutine()
    {
        yield return new WaitForSeconds(Random.Range(1f,3f));
        _meshRenderer.enabled = false;
    }
}