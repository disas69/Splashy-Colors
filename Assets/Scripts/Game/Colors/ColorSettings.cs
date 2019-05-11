using System;
using System.Collections.Generic;
using Framework.Attributes;
using Framework.Tools.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Colors
{
    [Serializable]
    public class ColorConfig
    {
        public string Name;
        public Color Color;
        public Material Material;
    }
    
    [ResourcePath("Settings/ColorSettings")]
    [CreateAssetMenu(fileName = "ColorSettings", menuName = "Game/ColorSettings")]
    public class ColorSettings : ScriptableSingleton<ColorSettings>
    {
        public List<ColorConfig> Configs = new List<ColorConfig>();

        public static string GetRandomColorName()
        {
            var index = Random.Range(0, Instance.Configs.Count);

            if (Instance.Configs.Count > 0)
            {
                return Instance.Configs[index].Name;
            }

            return string.Empty;
        }

        public static Color GetColor(string colorName)
        {
            var config = Instance.Configs.Find(c => c.Name == colorName);
            if (config != null)
            {
                return config.Color;
            }

            Debug.LogError($"Failed to find color by name: {colorName}");
            return Color.white;
        }

        public static Material GetMaterial(string colorName)
        {
            var config = Instance.Configs.Find(c => c.Name == colorName);
            if (config != null)
            {
                return config.Material;
            }

            Debug.LogError($"Failed to find material by name: {colorName}");
            return null;
        }
    }
}