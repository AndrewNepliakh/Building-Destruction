using UnityEngine;

namespace Managers
{
    public interface ISwitchablePart
    {
        void Init(DestroyableElement destroyableElement);
        public void SwitchOff();
    }
}