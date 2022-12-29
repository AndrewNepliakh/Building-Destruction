using System;
using UnityEngine;

namespace Managers
{
    public abstract class Window : MonoBehaviour, IUIView, IRestartable
    {
        public AssetsLoader AssetsLoader { get; set; }

        public virtual void Show(UIViewArguments arguments)
        {
            if (arguments.AssetsLoader != null)
                AssetsLoader = arguments.AssetsLoader;
            gameObject.SetActive(true);
        }

        public virtual void Hide(UIViewArguments arguments)
        {
            gameObject.SetActive(false);
        }

        public abstract void Reset();
    }
}