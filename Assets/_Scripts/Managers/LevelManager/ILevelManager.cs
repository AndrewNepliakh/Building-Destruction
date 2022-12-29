using System;
using System.Threading.Tasks;

namespace Managers
{
    public interface ILevelManager
    {
        ILevel CurrentLevel { get; }
        Task CreateLevel(LevelID id);
        Task CreateLevel();
        Task NextLevel();
        void UnloadLevels();
        event Action<ILevel> OnLevelLoaded;
    }
}