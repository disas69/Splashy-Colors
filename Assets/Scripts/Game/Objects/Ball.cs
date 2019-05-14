using Framework.Extensions;
using Framework.Input;
using Framework.Localization;
using Framework.Signals;
using Framework.UI.Notifications;
using Framework.UI.Notifications.Model;
using Framework.Utils.Math;
using Game.Data;
using Game.Main;
using Game.Path;
using Game.Pickups;
using Game.Spawn;
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
        [SerializeField] private string _failSound;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _dragSpeedAverage = new VectorAverager(0.1f);
            _screenToWorldScaleFactor = 2f * Camera.main.orthographicSize / Camera.main.pixelHeight;

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
            SignalsManager.Broadcast(_audioSignal.Name, _failSound);
            NotificationManager.Show(new TextNotification(LocalizationManager.GetString("Ouch")), 2f);
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
            
            _dragSpeedAverage.AddSample(_dragging ? _lastDragDelta : Vector2.zero);
            var velocity = _dragSpeedAverage.Value;

            if (velocity.magnitude > 0.01f)
            {
                var worldSpaceDelta = velocity * GameConfiguration.Instance.BallSettings.MoveSpeed * _screenToWorldScaleFactor;
                newPosition = Vector3.SmoothDamp(transform.position, transform.position + worldSpaceDelta, ref _velocity, GameConfiguration.Instance.BallSettings.SmoothSpeed);
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
                    newPosition.y = Mathf.Lerp(GameConfiguration.Instance.BallSettings.JumpHeight, 0f, GameConfiguration.Instance.BallSettings.InCurve.Evaluate(distance / halfWay));
                }
                else
                {
                    newPosition.y = Mathf.Lerp(0f, GameConfiguration.Instance.BallSettings.JumpHeight, GameConfiguration.Instance.BallSettings.OutCurve.Evaluate(distance / halfWay));
                }
            }

            transform.position = new Vector3(Mathf.Clamp(newPosition.x, -GameConfiguration.Instance.BallSettings.XPositionCap, GameConfiguration.Instance.BallSettings.XPositionCap), 
                Mathf.Clamp(newPosition.y, 0f, GameConfiguration.Instance.BallSettings.JumpHeight), newPosition.z);
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
                return;
            }

            var platform = otherCollider.GetComponent<Platform>();
            if (platform != null)
            {
                _isOnPlatform = true;
                ProcessPlatform(platform);
                return;
            }
            
            var pickup = otherCollider.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.Trigger();
                return;
            }

            var fallZone = otherCollider.GetComponent<FallZone>();
            if (fallZone != null && !_isOnPlatform)
            {
                ProcessFall();
                SignalsManager.Broadcast(_stateSignal.Name, GameState.End.ToString());
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

        private void ProcessFall()
        {
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(Vector3.down * GameConfiguration.Instance.BallSettings.MoveSpeed, ForceMode.Impulse);
            NotificationManager.Show(new TextNotification(LocalizationManager.GetString("Step")), 2f);
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