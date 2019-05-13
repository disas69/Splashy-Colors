using System;
using Game.Pickups;
using UnityEngine;

namespace Game.Data.Settings
{
    [Serializable]
    public class PickupsSettings
    {
        public string Name;
        public Pickup Prefab;
        [Range(0, 1)]
        public float Chance;
    }
}