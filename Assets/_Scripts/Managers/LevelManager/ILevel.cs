using System;
using UnityEngine;

namespace Managers
{
    public interface ILevel
    {
        Action<int> OnAttemptChanged { get; set; }
        LevelID LevelID { get; }
        AssetsLoader loader { get; set; }
        GameObject gameObject { get; }
        Transform transform { get; }
        int MaxAttempts { get; }
        Vector3 SpawnPoint { get; }
        void Init(object o);
        void UnloadLevel();
    }
}