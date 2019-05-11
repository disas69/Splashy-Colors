using UnityEngine;

namespace Game.Objects
{
    [CreateAssetMenu(fileName = "BallSettings", menuName = "Game/BallSettings")]
    public class BallSettings : ScriptableObject
    {
        public Vector3 StartPosition;
        public float JumpHeight;
        public float MoveSpeed;
        public float SmoothSpeed;
        public float VelocityCap;
        public float XPositionCap;
        public float BlotDeactivationTime;
        public AnimationCurve InCurve;
        public AnimationCurve OutCurve;
    }
}