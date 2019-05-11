using System;
using UnityEngine;

namespace Game.Objects
{
    [Serializable]
    public class BallSettings
    {
        public float JumpHeight;
        public float MoveSpeed;
        public float SmoothSpeed;
        public float XPositionCap;
        public AnimationCurve InCurve;
        public AnimationCurve OutCurve;
    }
}