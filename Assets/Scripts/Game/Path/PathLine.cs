using System.Collections.Generic;
using Game.Main;
using Game.Objects;
using Game.Spawn;
using UnityEngine;

namespace Game.Path
{
    [RequireComponent(typeof(Spawner))]
    public class PathLine : SpawnableObject
    {
        private Spawner _platformSpawner;
        private List<Platform> _platforms;

        [SerializeField] private PathLineSettings _settings;

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

        public void Setup(bool isFirstPlatform, string color)
        {
            var count = isFirstPlatform ? 1 : Random.Range(_settings.MinPlatmormsCount, _settings.MaxPlatformsCount + 1);
            var width = count * _settings.PlatformWidth;
            var position = Vector3.zero;
            position.x = -(width / 2f);

            for (var i = 0; i < count; i++)
            {
                position.x += _settings.PlatformWidth / 2f;

                var platform = _platformSpawner.Spawn() as Platform;
                if (platform != null)
                {
                    platform.transform.localPosition = position;
                    _platforms.Add(platform);
                }

                position.x += _settings.PlatformWidth / 2f;
            }
            
            ApplyColor(color);
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
    }
}