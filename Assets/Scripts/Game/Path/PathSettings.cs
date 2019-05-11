using UnityEngine;

namespace Game.Path
{
    [CreateAssetMenu(fileName = "PathSettings", menuName = "Game/PathSettings")]
    public class PathSettings : ScriptableObject
    {
        public float StartSpeed;
        public float SpeedMultiplier;
        public float SpeedIncreaseTime;
        public float MinPlatformDistance;
        public float MaxPlatformDistance;
        public float MaxXShift;
    }
}