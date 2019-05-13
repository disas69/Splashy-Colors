using System.Collections.Generic;
using Game.Data;
using Game.Data.Settings;
using Game.Objects;
using Game.Pickups;
using Game.Spawn;
using UnityEngine;

namespace Game.Path
{
    [RequireComponent(typeof(Spawner))]
    public class Line : SpawnableObject
    {
        private Spawner _platformSpawner;
        private List<Platform> _platforms;

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        private void Awake()
        {
            _platformSpawner = GetComponent<Spawner>();
            _platforms = new List<Platform>();
        }

        public override void Deactivate()
        {
            for (var i = 0; i < _platforms.Count; i++)
            {
                _platforms[i].Deactivate();
            }

            _platforms.Clear();
            base.Deactivate();
        }

        public void Setup(bool isFirstPlatform, LevelSettings level, string color, Pickup pickup = null)
        {
            var count = isFirstPlatform ? 1 : Random.Range(level.LineSettings.MinPlatformsCount, level.LineSettings.MaxPlatformsCount + 1);
            var width = count * level.LineSettings.PlatformWidth;
            var position = Vector3.zero;
            position.x = -(width / 2f);

            for (var i = 0; i < count; i++)
            {
                position.x += level.LineSettings.PlatformWidth / 2f;

                var platform = _platformSpawner.Spawn() as Platform;
                if (platform != null)
                {
                    platform.transform.localPosition = position;
                    _platforms.Add(platform);
                }

                position.x += level.LineSettings.PlatformWidth / 2f;
            }

            ApplyColor(color);

            if (!isFirstPlatform && pickup != null)
            {
                SpawnPickup(pickup);
            }
        }

        public void ApplyColor(string color)
        {
            if (_platforms.Count == 1)
            {
                _platforms[0].ApplyColor(color);
            }
            else
            {
                var platform = _platforms[Random.Range(0, _platforms.Count)];
                platform.ApplyColor(color);

                for (var i = 0; i < _platforms.Count; i++)
                {
                    var randomPlatform = _platforms[i];
                    if (randomPlatform != platform)
                    {
                        randomPlatform.ApplyColor(GameConfiguration.GetRandomColorName());
                    }
                }
            }
        }

        public void SpawnPickup(Pickup pickup)
        {
            var platform = _platforms[Random.Range(0, _platforms.Count)];
            pickup.Place(platform.BaseTransform.position, platform.BaseTransform);
            pickup.Activate();
        }
    }
}