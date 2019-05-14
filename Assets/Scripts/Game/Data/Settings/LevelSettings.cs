using System;

namespace Game.Data.Settings
{
    [Serializable]
    public class LevelSettings
    {
        public int Level;
        public int Score;
        public LineSettings LineSettings;
        public PathSettings PathSettings;

        public void Copy(LevelSettings levelSettings)
        {
            Level = levelSettings.Level;
            Score = levelSettings.Score;
            
            LineSettings = new LineSettings
            {
                PlatformScore = levelSettings.LineSettings.PlatformScore,
                ScoreMultiplier = levelSettings.LineSettings.ScoreMultiplier,
                PickupSpawnStep = levelSettings.LineSettings.PickupSpawnStep,
                PlatformWidth = levelSettings.LineSettings.PlatformWidth,
                MinPlatformsCount = levelSettings.LineSettings.MinPlatformsCount,
                MaxPlatformsCount = levelSettings.LineSettings.MaxPlatformsCount,
            };
            
            PathSettings = new PathSettings
            {
                StartSpeed = levelSettings.PathSettings.StartSpeed,
                MaxSpeed = levelSettings.PathSettings.MaxSpeed,
                SpeedMultiplier = levelSettings.PathSettings.SpeedMultiplier,
                SpeedIncreaseTime = levelSettings.PathSettings.SpeedIncreaseTime,
                MinPlatformDistance = levelSettings.PathSettings.MinPlatformDistance,
                MaxPlatformDistance = levelSettings.PathSettings.MaxPlatformDistance,
                MaxXShift = levelSettings.PathSettings.MaxXShift,
            };
        }
    }
}