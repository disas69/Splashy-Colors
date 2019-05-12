using System;
using Game.Pickups;
using UnityEngine;

namespace Game.Data.Settings
{
    [Serializable]
    public class PickupsSettings
    {
        public PickupType Type;
        [Range(0, 1)]
        public float Chance;
    }
}