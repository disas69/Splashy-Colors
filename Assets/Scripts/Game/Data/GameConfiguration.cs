using System.Collections.Generic;
using Framework.Attributes;
using Framework.Tools.Singleton;
using Game.Data.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Data
{
    [ResourcePath("GameConfiguration")]
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Game/GameConfiguration")]
    public class GameConfiguration : ScriptableSingleton<GameConfiguration>
    {
        [Range(1, 5)] 
        public int Lives;
        public int LinesCount;
        public float LinesVisibleRange;
        public float ColorChangeTime;
        public BallSettings BallSettings;
        public List<ColorSettings> Colors = new List<ColorSettings>();
        public List<PickupsSettings> Pickups = new List<PickupsSettings>();
        public List<LevelSettings> Levels = new List<LevelSettings>();

        public static string GetRandomColorName()
        {
            var index = Random.Range(0, Instance.Colors.Count);

            if (Instance.Colors.Count > 0)
            {
                return Instance.Colors[index].Name;
            }

            return string.Empty;
        }

        public static Color GetColor(string colorName)
        {
            var settings = Instance.Colors.Find(c => c.Name == colorName);
            if (settings != null)
            {
                return settings.Color;
            }

            Debug.LogError(string.Format("Failed to find color by name: {0}", colorName));
            return Color.white;
        }

        public static Material GetMaterial(string colorName)
        {
            var settings = Instance.Colors.Find(c => c.Name == colorName);
            if (settings != null)
            {
                return settings.Material;
            }

            Debug.LogError(string.Format("Failed to find material by name: {0}", colorName));
            return null;
        }

        public static LevelSettings GetLevelSettings(int level)
        {
            var settings = Instance.Levels.Find(l => l.Level == level);
            if (settings != null)
            {
                return settings;
            }

            if (Instance.Levels.Count > 0)
            {
                return Instance.Levels[Instance.Levels.Count - 1];
            }

            Debug.LogError(string.Format("Failed to find level settings for level: {0}", level));
            return null;
        }
        
        public static int GetLevelByScore(int score)
        {
            var settings = Instance.Levels.Find(l => l.Score > score);
            if (settings != null)
            {
                return settings.Level;
            }

            if (Instance.Levels.Count > 0)
            {
                return Instance.Levels[Instance.Levels.Count - 1].Level;
            }

            Debug.LogError(string.Format("Failed to find level for score: {0}", score));
            return 1;
        }
    }
}