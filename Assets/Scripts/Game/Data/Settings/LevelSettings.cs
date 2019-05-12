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
    }
}