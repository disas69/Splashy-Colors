using System;

namespace Game.Spawn
{
    [Serializable]
    public class SpawnerSettings
    {
        public SpawnableObject ObjectPrefab;
        public int PoolCapacity;
    }
}