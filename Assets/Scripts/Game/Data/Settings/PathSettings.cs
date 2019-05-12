using System;

namespace Game.Data.Settings
{
    [Serializable]
    public class PathSettings
    {
        public float StartSpeed = 5;
        public float MaxSpeed = 5;
        public float SpeedMultiplier;
        public float SpeedIncreaseTime;
        public float MinPlatformDistance = 3;
        public float MaxPlatformDistance = 3;
        public float MaxXShift = 2;
    }
}