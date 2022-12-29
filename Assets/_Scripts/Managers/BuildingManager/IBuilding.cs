using UnityEngine;

namespace Managers
{
    public interface IBuilding
    {
        AssetsLoader loader { get; set; }
        GameObject gameObject { get; }
        Transform transform { get; }
        void Init(object o);
        int GetLevelResult();
    }
}