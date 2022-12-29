using UnityEngine;

namespace Managers
{
    public class Popup : MonoBehaviour, IUIView
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
    }
}