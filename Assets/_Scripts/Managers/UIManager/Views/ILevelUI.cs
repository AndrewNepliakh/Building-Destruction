using UnityEngine.UI;

namespace Managers
{
    internal interface ILevelUI
    {
        int Index { get; }
        Button SelectButton { get; }

        void Init(LevelUIArgs args);
    }
}