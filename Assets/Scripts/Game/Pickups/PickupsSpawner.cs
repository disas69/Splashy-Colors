using System;
using System.Collections.Generic;
using Game.Data;
using Game.Spawn;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Pickups
{
    [Serializable]
    public class PickupSpawn
    {
        public PickupType Type;
        public Spawner Spawner;
    }
    
    public class PickupsSpawner : MonoBehaviour
    {
        public List<PickupSpawn> Spawners = new List<PickupSpawn>();

        public Pickup GetNextPickup()
        {
            var random = Random.value;
            var pickupSettings = GameConfiguration.Instance.Pickups;

            for (var i = 0; i < pickupSettings.Count; i++)
            {
                var pickupsSettings = pickupSettings[i];
                if (pickupsSettings.Chance >= random)
                {
                    var pickupSpawn = Spawners.Find(s => s.Type == pickupsSettings.Type);
                    if (pickupSpawn != null)
                    {
                        return pickupSpawn.Spawner.Spawn() as Pickup;
                    }
                }
            }

            return null;
        }
    }
}