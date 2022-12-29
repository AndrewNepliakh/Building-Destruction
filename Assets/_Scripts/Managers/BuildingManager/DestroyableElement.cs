using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class DestroyableElement : MonoBehaviour
    {
        public bool IsDestroyed { get; private set; }

        [SerializeField] private GameObject _elment;
        [SerializeField] private GameObject _brokenElment;
        
        private IntactPart _intactPart; 
        private BrokenPart _brokenPart;
        
        [ContextMenu("Construct")]
        private void Construct()
        {
            _elment = transform.GetChild(0).gameObject;
            _brokenElment = transform.GetChild(1).gameObject;

            if (_elment.TryGetComponent<BoxCollider>(out var collider))
            {
                DestroyImmediate(collider);
            }
            
            if (_elment.TryGetComponent<IntactPart>(out var intactPart))
            {
                DestroyImmediate(intactPart);
            }

            _elment.AddComponent<BoxCollider>();
            _elment.AddComponent<IntactPart>();
            _elment.GetComponent<IntactPart>().Init(this);

            if (_brokenElment.TryGetComponent<BrokenPart>(out var brokenPart))
            {
                DestroyImmediate(brokenPart);
            }
            
            _brokenElment.AddComponent<BrokenPart>();
            _brokenElment.GetComponent<BrokenPart>().Init();
            _brokenElment.gameObject.SetActive(false);
        }

        private void Start()
        {
            _intactPart = _elment.GetComponent<IntactPart>();
            _brokenPart = _brokenElment.GetComponent<BrokenPart>();
        }

        public void Prepare()
        {
            IsDestroyed = true;
            _elment.gameObject.SetActive(false);
            _brokenElment.gameObject.SetActive(true);
        }
    }
}