using System.Collections.Generic;
using Framework.Attributes;
using Framework.Tools.Singleton;
using Game.Objects;
using UnityEngine;

namespace Game.Main
{
    [ResourcePath("GameConfiguration")]
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Game/GameConfiguration")]
    public class GameConfiguration : ScriptableSingleton<GameConfiguration>
    {
        [Range(1, 5)] public int Lives;
        public int LinesVisible;
        public float LinesDeactivationDistance;
        public BallSettings BallSettings;
        public List<ColorSettings> Colors = new List<ColorSettings>();

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
            var config = Instance.Colors.Find(c => c.Name == colorName);
            if (config != null)
            {
                return config.Color;
            }

            Debug.LogError($"Failed to find color by name: {colorName}");
            return Color.white;
        }

        public static Material GetMaterial(string colorName)
        {
            var config = Instance.Colors.Find(c => c.Name == colorName);
            if (config != null)
            {
                return config.Material;
            }

            Debug.LogError($"Failed to find material by name: {colorName}");
            return null;
        }
    }
}