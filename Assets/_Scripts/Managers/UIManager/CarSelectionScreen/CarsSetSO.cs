using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Car Set")]
public class CarsSetSO : ScriptableObject, IEnumerable<PlayerCarSO>
{
    public List<PlayerCarSO> Cars;
    public IEnumerator<PlayerCarSO> GetEnumerator() => Cars.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}