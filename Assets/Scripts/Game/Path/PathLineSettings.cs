using UnityEngine;

namespace Game.Path
{
    [CreateAssetMenu(fileName = "PathLineSettings", menuName = "Game/PathLineSettings")]
    public class PathLineSettings : ScriptableObject
    {
        public float PlatformWidth;
        public int MinPlatmormsCount;
        public int MaxPlatformsCount;
    }
}