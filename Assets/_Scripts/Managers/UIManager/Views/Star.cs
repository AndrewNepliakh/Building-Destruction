using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private GameObject _star;
        [SerializeField] private GameObject _starGrey;

        public void Activate()
        {
            _star.SetActive(true);
            _starGrey.SetActive(false);
        }
        
        public void Deactivate()
        {
            _star.SetActive(false);
            _starGrey.SetActive(true);
        }
    }
}