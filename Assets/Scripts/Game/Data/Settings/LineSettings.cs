using System;

namespace Game.Data.Settings
{
    [Serializable]
    public class LineSettings
    {
        public int PlatformScore = 1;
        public int ScoreMultiplier = 2;
        public int PickupSpawnStep;
        public float PlatformWidth = 4;
        public int MinPlatformsCount = 1;
        public int MaxPlatformsCount = 1;
    }
}