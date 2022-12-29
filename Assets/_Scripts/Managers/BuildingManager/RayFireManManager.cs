using System.Collections;
using System.Collections.Generic;
using RayFire;
using UnityEngine;

public class RayFireManManager : MonoBehaviour
{
    private GameObject _rayFireManRoot;
    public void Init()
    {
        if (_rayFireManRoot != null)
        {
            Destroy(_rayFireManRoot.gameObject);
        }

        StartCoroutine(CreateNewOne());

    }

    IEnumerator CreateNewOne()
    {
        yield return null;
        _rayFireManRoot = new GameObject("[RayFireMan]");
        _rayFireManRoot.AddComponent<RayfireMan>();
    }
}
