using System;
using UnityEngine;

namespace _Scripts.Player
{
    public class ColorController : MonoBehaviour
    {
        public int Index;
        
        [SerializeField] private MeshRenderer renderer;

#if UNITY_EDITOR
        private void OnValidate()
        {
            renderer = GetComponentInChildren<MeshRenderer>();
        }
#endif
        
        public void TurnGreyColor()
        {
            renderer.material.SetFloat("_Saturation", 0.0f);
            renderer.material.SetFloat("_Brightness", 0.3f);
        }
        
        public void ResetColor()
        {
            renderer.material.SetFloat("_Saturation", 1.0f);
            renderer.material.SetFloat("_Brightness", 1.0f);
        }
    }
    
    [Serializable]
    public enum ColorCondition
    {
        TurnGrey,
        Reset
    }
}