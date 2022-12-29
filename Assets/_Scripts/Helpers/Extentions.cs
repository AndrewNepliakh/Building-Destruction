using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions 
{
   public static Coroutine Start(this Coroutine coroutine, MonoBehaviour monoBehaviour, IEnumerator enumerator)
   {
      if (coroutine == null)
         coroutine = monoBehaviour.StartCoroutine(enumerator);
      else
      {
         monoBehaviour.StopCoroutine(coroutine);
         coroutine = monoBehaviour.StartCoroutine(enumerator);
      }

      return coroutine;
   }
   
    public static void Random(this ref Vector3 myVector, Vector3 min, Vector3 max)
     {
         myVector = new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
     }

    public static T GetNearest<T>(this List<T> list, Transform target) where T : MonoBehaviour
    {
       var nearestDistance = float.PositiveInfinity;
       T nearestItem = null;

       foreach (var item in list)
       {
          var distance = Vector3.Distance(item.transform.position, target.position);
          if (!(distance < nearestDistance)) continue;
          nearestDistance = distance;
          nearestItem = item;
       }
      
       return nearestItem;
    }
}
