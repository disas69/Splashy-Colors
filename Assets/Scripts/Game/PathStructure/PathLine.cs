using System.Collections.Generic;
using Game.PlatformStructure;
using Game.SpawnStructure;
using UnityEngine;

namespace Game.PathStructure
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
            base.Deactivate();

            for (var i = 0; i < _platforms.Count; i++)
            {
                _platforms[i].Deactivate();
            }
        }

        public void Setup(bool isFirstPlatform)
        {
            var count = isFirstPlatform ? 1 : Random.Range(_settings.MinPlatmormsCount, _settings.MaxPlatformsCount + 1);
            var width = count * _settings.PlatformWidth;
            var position = _settings.StartPosition;
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
        }
    }
}