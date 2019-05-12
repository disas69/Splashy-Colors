using System.Collections.Generic;
using Framework.Signals;
using Game.Data;
using Game.Data.Settings;
using Game.Main;
using Game.Spawn;
using UnityEngine;

namespace Game.Path
{
    [RequireComponent(typeof(Spawner))]
    public class PathController : MonoBehaviour
    {
        private bool _isActive;
        private float _speed;
        private float _time;
        private float _timeSinceLastSpeedIncrease;
        private Spawner _linesSpawner;
        private List<Line> _lines;
        private string _color;

        [SerializeField] private Signal _levelSignal;

        private void Awake()
        {
            _linesSpawner = GetComponent<Spawner>();
            _lines = new List<Line>();
        }

        public void ResetPath()
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                _lines[i].Deactivate();
            }

            _lines.Clear();

            for (var i = 0; i < GameConfiguration.Instance.LinesCount; i++)
            {
                SpawnLine();
            }
        }

        public void Activate()
        {
            _isActive = true;
            _time = 0f;
            _timeSinceLastSpeedIncrease = Time.time;
            
            ApplyStartSpeed(GameController.Instance.GameSession.Level);
            SignalsManager.Register(_levelSignal.Name, ApplyStartSpeed);
        }

        public void Deactivate()
        {
            _isActive = false;
            SignalsManager.Unregister(_levelSignal.Name, ApplyStartSpeed);
        }

        public void ApplyColor(string color)
        {
            _color = color;

            for (var i = 0; i < _lines.Count; i++)
            {
                _lines[i].ApplyColor(color);
            }
        }

        public Line GetNextPathLine(Line current)
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

            while (_lines.Count < GameConfiguration.Instance.LinesCount)
            {
                SpawnLine();
            }

            for (var i = _lines.Count - 1; i >= 0; i--)
            {
                var line = _lines[i];
                if (line.Position.z < -GameConfiguration.Instance.LinesVisibleRange)
                {
                    line.Deactivate();
                    _lines.RemoveAt(i);
                }
                else
                {
                    var levelSettings = GameConfiguration.GetLevelSettings(GameController.Instance.GameSession.Level);
                    if (levelSettings != null)
                    {
                        if (levelSettings.PathSettings.SpeedIncreaseTime > 0f && Time.time - _timeSinceLastSpeedIncrease > levelSettings.PathSettings.SpeedIncreaseTime)
                        {
                            _speed = Mathf.Clamp(_speed * (1 + levelSettings.PathSettings.SpeedMultiplier), levelSettings.PathSettings.StartSpeed, levelSettings.PathSettings.MaxSpeed);
                            _timeSinceLastSpeedIncrease = Time.time;
                            Debug.Log($"Path speed change: {_speed}");
                        }
                        
                        line.Position += Vector3.back * _speed * Time.deltaTime;
                    }
                }
            }

            _time += Time.deltaTime;
        }

        private void SpawnLine()
        {
            var level = GameConfiguration.GetLevelSettings(GameController.Instance.GameSession.Level);
            if (level != null)
            {
                var line = _linesSpawner.Spawn() as Line;
                if (line != null)
                {
                    var position = Vector3.zero;

                    if (_lines.Count > 0)
                    {
                        position = _lines[_lines.Count - 1].transform.position;
                        position.x = Random.Range(-level.PathSettings.MaxXShift, level.PathSettings.MaxXShift);
                        position.z += Random.Range(level.PathSettings.MinPlatformDistance, level.PathSettings.MaxPlatformDistance);
                    }

                    line.Position = position;
                    line.Setup(_lines.Count == 0, _color);

                    _lines.Add(line);
                }
            }
        }

        private void ApplyStartSpeed(int level)
        {
            var levelSettings = GameConfiguration.GetLevelSettings(level);
            if (levelSettings != null)
            {
                _speed = levelSettings.PathSettings.StartSpeed;
            }
        }
    }
}