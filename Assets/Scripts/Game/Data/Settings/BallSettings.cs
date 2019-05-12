using System;
using UnityEngine;

namespace Game.Data.Settings
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