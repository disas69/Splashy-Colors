using System;

namespace Game.Data.Settings
{
    [Serializable]
    public class PathSettings
    {
        public float StartSpeed;
        public float MaxSpeed;
        public float SpeedMultiplier;
        public float SpeedIncreaseTime;
        public float MinPlatformDistance;
        public float MaxPlatformDistance;
        public float MaxXShift;
    }
}