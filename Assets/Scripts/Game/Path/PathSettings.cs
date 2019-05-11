using UnityEngine;

namespace Game.Path
{
    [CreateAssetMenu(fileName = "PathSettings", menuName = "Game/PathSettings")]
    public class PathSettings : ScriptableObject
    {
        public Vector3 StartPosition;
        public float StartSpeed;
        public float SpeedMultiplier;
        public float MinPlatformDistance;
        public float MaxPlatformDistance;
        public float MaxXShift;
        public int VisibleLinesCount;
        public float DeactivationDistance;
    }
}