using System.Collections.Generic;
using Game.Spawn;
using UnityEngine;

namespace Game.Path
{
    [RequireComponent(typeof(Spawner))]
    public class PathController : MonoBehaviour
    {
        private bool _isActive;
        private Spawner _linesSpawner;
        private List<PathLine> _lines;
        private string _color;

        [SerializeField] private PathSettings _pathSettings;

        private void Awake()
        {
            _linesSpawner = GetComponent<Spawner>();
            _lines = new List<PathLine>();
        }

        public void ResetPath()
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                _lines[i].Deactivate();
            }

            _lines.Clear();
            _linesSpawner.Flush();

            for (var i = 0; i < _pathSettings.VisibleLinesCount; i++)
            {
                SpawnLine();
            }
        }

        public void Activate()
        {
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }

        public void ApplyColor(string color)
        {
            _color = color;

            for (var i = 0; i < _lines.Count; i++)
            {
                _lines[i].ApplyColor(color);
            }
        }

        public PathLine GetNextPathLine(PathLine current)
        {
            var index = _lines.FindIndex(p => p == current);
            if (index + 1 < _lines.Count)
            {
                return _lines[index + 1];
            }

            return null;
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            while (_lines.Count < _pathSettings.VisibleLinesCount)
            {
                SpawnLine();
            }

            for (var i = _lines.Count - 1; i >= 0; i--)
            {
                var line = _lines[i];
                if (line.Position.z < -_pathSettings.DeactivationDistance)
                {
                    line.Deactivate();
                    _lines.RemoveAt(i);
                }
                else
                {
                    line.Position += Vector3.back * _pathSettings.StartSpeed * Time.deltaTime;
                }
            }
        }

        private void SpawnLine()
        {
            var line = _linesSpawner.Spawn() as PathLine;
            if (line != null)
            {
                var position = _pathSettings.StartPosition;

                if (_lines.Count > 0)
                {
                    position = _lines[_lines.Count - 1].transform.position;
                    position.x = Random.Range(-_pathSettings.MaxXShift, _pathSettings.MaxXShift);
                    position.z += Random.Range(_pathSettings.MinPlatformDistance, _pathSettings.MaxPlatformDistance);
                }

                line.Position = position;
                line.Setup(_lines.Count == 0, _color);

                _lines.Add(line);
            }
        }
    }
}