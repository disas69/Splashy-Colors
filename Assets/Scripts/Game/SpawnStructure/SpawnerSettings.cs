using System;

namespace Game.SpawnStructure
{
    [Serializable]
    public class SpawnerSettings
    {
        public SpawnableObject ObjectPrefab;
        public int PoolCapacity;
    }
}