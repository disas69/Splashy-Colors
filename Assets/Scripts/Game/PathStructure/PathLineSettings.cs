using UnityEngine;

namespace Game.PathStructure
{
    [CreateAssetMenu(fileName = "PathLineSettings", menuName = "Game/PathLineSettings")]
    public class PathLineSettings : ScriptableObject
    {
        public Vector3 StartPosition;
        public float PlatformWidth;
        public int MinPlatmormsCount;
        public int MaxPlatformsCount;
    }
}