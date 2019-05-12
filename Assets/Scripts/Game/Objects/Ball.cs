﻿using System.Collections;
using Framework.Extensions;
using Framework.Input;
using Framework.Signals;
using Framework.Utils.Math;
using Game.Data;
using Game.Data.Settings;
using Game.Main;
using Game.Path;
using Game.Pickups;
using Game.Spawn;
using Game.UI;
using Game.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ball : MonoBehaviour
    {
        private bool _isActive;
        private bool _isOnPlatform;
        private bool _isInvincible;
        private VectorAverager _dragSpeedAverage;
        private float _screenToWorldScaleFactor;
        private Line _currentLine;
        private Vector3 _velocity;
        private bool _dragging;
        private int _pointersCount;
        private Vector2 _lastDragDelta;
        private Vector2 _currentVelocity;
        private Rigidbody _rigidbody;
        private string _color;

        [SerializeField] private InputEventProvider _inputProvider;
        [SerializeField] private PathController _path;
        [SerializeField] private Spawner _blotSpawner;
        [SerializeField] private Spawner _hintsSpawner;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private ColorChanger _colorChanger;
        [SerializeField] private Signal _stateSignal;
        [SerializeField] private Signal _audioSignal;

        public string Color => _color;
        public BallSettings Settings => GameConfiguration.Instance.BallSettings;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _dragSpeedAverage = new VectorAverager(0.1f);
            _screenToWorldScaleFactor = 2 * Camera.main.orthographicSize / Camera.main.pixelHeight;

            _inputProvider.PointerDown += OnPointerDown;
            _inputProvider.PointerUp += OnPointerUp;
            _inputProvider.BeginDrag += OnBeginDrag;
            _inputProvider.Drag += OnDrag;
        }

        public void ResetBall()
        {
            _isOnPlatform = true;
            _blotSpawner.Flush();
            _hintsSpawner.Flush();
            _renderer.enabled = true;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            transform.position = Vector3.zero;
        }

        public void Activate()
        {
            _isActive = true;
        }

        public void Deactivate()
        {
            _isActive = false;
        }

        public void BlowUp()
        {
            _renderer.enabled = false;
            SignalsManager.Broadcast(_audioSignal.Name, "tap");
        }

        public void ApplyColor(string color)
        {
            _isInvincible = true;
            _color = color;
            _colorChanger.ChangeColor(GameConfiguration.GetMaterial(color), GameConfiguration.Instance.ColorChangeTime);

            this.WaitForSeconds(GameConfiguration.Instance.BallSettings.InvincibilityTime, () => _isInvincible = false);
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            _pointersCount++;
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            _dragSpeedAverage.Clear();
        }

        private void OnDrag(PointerEventData eventData)
        {
            _lastDragDelta = eventData.delta;
        }

        private void OnPointerUp(PointerEventData eventData)
        {
            _pointersCount--;
            if (_pointersCount == 0 && _dragging)
            {
                _dragging = false;
            }
        }

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }

            var newPosition = transform.position;

            if (_dragging)
            {
                _currentVelocity = _lastDragDelta;
                _dragSpeedAverage.AddSample(_currentVelocity);
            }
            else
            {
                _currentVelocity = Vector2.zero;
            }

            if (_currentVelocity.magnitude > 0.01f)
            {
                Vector3 worldSpaceDelta = _currentVelocity * Settings.MoveSpeed * _screenToWorldScaleFactor;
                newPosition = Vector3.SmoothDamp(transform.position, transform.position + worldSpaceDelta, ref _velocity, Settings.SmoothSpeed);
            }

            _lastDragDelta = Vector2.zero;

            var nextPathLine = GetNextPathLine();
            if (nextPathLine != null)
            {
                var distance = Mathf.Abs(transform.position.z - nextPathLine.Position.z);
                var halfWay = GetHalfWay(nextPathLine);

                if (distance > halfWay)
                {
                    distance -= halfWay;
                    newPosition.y = Mathf.Lerp(Settings.JumpHeight, 0f, Settings.InCurve.Evaluate(distance / halfWay));
                }
                else
                {
                    newPosition.y = Mathf.Lerp(0f, Settings.JumpHeight, Settings.OutCurve.Evaluate(distance / halfWay));
                }
            }

            transform.position = new Vector3(Mathf.Clamp(newPosition.x, -Settings.XPositionCap, Settings.XPositionCap),
                Mathf.Clamp(newPosition.y, 0f, Settings.JumpHeight), newPosition.z);
        }

        private float GetHalfWay(Line nextLine)
        {
            return (nextLine.Position.z - _currentLine.Position.z) / 2f;
        }

        private Line GetNextPathLine()
        {
            return _path.GetNextPathLine(_currentLine);
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            var pathLine = otherCollider.GetComponent<Line>();
            if (pathLine != null)
            {
                _currentLine = pathLine;
            }
            else
            {
                var platform = otherCollider.GetComponent<Platform>();
                if (platform != null)
                {
                    _isOnPlatform = true;
                    ProcessPlatform(platform);
                }
                else
                {
                    var fallZone = otherCollider.GetComponent<FallZone>();
                    if (fallZone != null && !_isOnPlatform)
                    {
                        _rigidbody.useGravity = true;
                        _rigidbody.isKinematic = false;
                        _rigidbody.AddForce(Vector3.down * Settings.MoveSpeed, ForceMode.Impulse);
                        SignalsManager.Broadcast(_stateSignal.Name, GameState.End.ToString());
                    }
                }
            }

            var pickup = otherCollider.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.Trigger();
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            var platform = otherCollider.GetComponent<Platform>();
            if (platform != null)
            {
                _isOnPlatform = false;
            }
        }

        private void ProcessPlatform(Platform platform)
        {
            if (platform.Color == _color)
            {
                if (GameController.Instance.GameState == GameState.Play)
                {
                    var level = GameConfiguration.GetLevelSettings(GameController.Instance.GameSession.Level);
                    if (level != null)
                    {
                        var score = level.LineSettings.PlatformScore * GameController.Instance.GameSession.ScoreMultiplier;
                        GameController.Instance.GameSession.AddScorePoints(score);
                        
                        var hint = _hintsSpawner.Spawn() as Hint;
                        if (hint != null)
                        {
                            hint.Place(transform.position, platform.BaseTransform, score);
                        }
                    }
                }
            }
            else
            {
                if (!_isInvincible)
                {
                    GameController.Instance.GameSession.SubtractLive();
                }
            }
            
            var blot = _blotSpawner.Spawn() as Blot;
            if (blot != null)
            {
                blot.Place(transform.position, platform.BaseTransform, _color);
            }
            
            platform.Trigger();
        }

        private void OnDestroy()
        {
            _inputProvider.PointerDown -= OnPointerDown;
            _inputProvider.PointerUp -= OnPointerUp;
            _inputProvider.BeginDrag -= OnBeginDrag;
            _inputProvider.Drag -= OnDrag;
        }
    }
}