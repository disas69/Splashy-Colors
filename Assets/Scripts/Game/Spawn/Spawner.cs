using System;
using System.Collections.Generic;
using Framework.Tools.Gameplay;
using UnityEngine;

namespace Game.Spawn
{
    [Serializable]
    public class Spawner : MonoBehaviour
    {
        private Pool<SpawnableObject> _objectsPool;
        private List<SpawnableObject> _activeObjects;

        [SerializeField] private bool _activateOnAwake = true;
        [SerializeField] private SpawnerSettings _settings;
        
        public int Count
        {
            get { return _activeObjects.Count; }
        }

        private void Awake()
        {
            _activeObjects = new List<SpawnableObject>();

            if (_activateOnAwake)
            {
                Activate(_settings);
            }
        }

        public void Activate(SpawnerSettings spawnerSettings)
        {
            if (_objectsPool == null)
            {
                _objectsPool = new Pool<SpawnableObject>(spawnerSettings.ObjectPrefab, transform, spawnerSettings.PoolCapacity);
            }
        }

        public SpawnableObject Spawn()
        {
            var spawnableObject = _objectsPool.GetNext();
            spawnableObject.Deactivated += OnObjectDeactivated;
            _activeObjects.Add(spawnableObject);
            
            return spawnableObject;
        }

        public void Flush()
        {
            for (int i = 0; i < _activeObjects.Count; i++)
            {
                Despawn(_activeObjects[i]);
            }
        }

        private void OnObjectDeactivated(SpawnableObject spawnableObject)
        {
            Despawn(spawnableObject);
        }

        private void Despawn(SpawnableObject spawnableObject)
        {
            spawnableObject.Deactivated -= OnObjectDeactivated;

            _activeObjects.Remove(spawnableObject);
            _objectsPool.Return(spawnableObject);
        }
    }
}