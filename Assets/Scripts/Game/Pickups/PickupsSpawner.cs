using System;
using System.Collections.Generic;
using Game.Data;
using Game.Data.Settings;
using Game.Spawn;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Pickups
{
    [Serializable]
    public class PickupSpawn
    {
        public string Name;
        public Spawner Spawner;
    }

    public class PickupsSpawner : MonoBehaviour
    {
        private List<PickupSpawn> _spawners = new List<PickupSpawn>();

        [SerializeField] private int _poolsCapacity;

        private void Awake()
        {
            var pickupSettings = GameConfiguration.Instance.Pickups;

            for (var i = 0; i < pickupSettings.Count; i++)
            {
                var pickup = pickupSettings[i];
                var spawnSettings = new SpawnerSettings
                {
                    ObjectPrefab = pickup.Prefab, PoolCapacity = _poolsCapacity
                };

                var spawner = new GameObject(string.Format("Spawner [{0}]", pickup.Name)).AddComponent<Spawner>();
                spawner.transform.SetParent(transform);
                spawner.Activate(spawnSettings);

                _spawners.Add(new PickupSpawn {Name = pickup.Name, Spawner = spawner});
            }
        }

        public Pickup GetNextPickup()
        {
            var random = Random.value;
            var pickupSettings = GameConfiguration.Instance.Pickups;

            for (var i = 0; i < pickupSettings.Count; i++)
            {
                var pickupsSettings = pickupSettings[i];
                if (pickupsSettings.Chance >= random)
                {
                    var pickupSpawn = _spawners.Find(s => s.Name == pickupsSettings.Name);
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