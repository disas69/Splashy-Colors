using Framework.Extensions;
using Framework.Input;
using Framework.Signals;
using Framework.Utils.Math;
using Game.Colors;
using Game.Main;
using Game.Path;
using Game.Spawn;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Objects
{
    [RequireComponent(typeof(Rigidbody), typeof(Spawner))]
    public class Ball : MonoBehaviour
    {
        private bool _isActive;
        private bool _isOnPlatform;
        private VectorAverager _dragSpeedAverage;
        private float _screenToWorldScaleFactor;
        private PathLine _currentPathLine;
        private Vector3 _velocity;
        private bool _dragging;
        private int _pointersCount;
        private Vector2 _lastDragDelta;
        private Vector2 _currentVelocity;
        private Rigidbody _rigidbody;
        private Spawner _blotSpawner;
        private string _color;

        [SerializeField] private InputEventProvider _inputProvider;
        [SerializeField] private PathController _path;
        [SerializeField] private BallSettings _settings;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Signal _stateSignal;
        [SerializeField] private Signal _audioSignal;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _blotSpawner = GetComponent<Spawner>();
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
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            _blotSpawner.Flush();
            transform.position = _settings.StartPosition;
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
            _renderer.sharedMaterial = ColorSettings.GetMaterial(color);
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
                Vector3 worldSpaceDelta = _currentVelocity * _settings.MoveSpeed * _screenToWorldScaleFactor;
                newPosition = Vector3.SmoothDamp(transform.position, transform.position + worldSpaceDelta, ref _velocity, _settings.SmoothSpeed);
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
                    newPosition.y = Mathf.Lerp(_settings.JumpHeight, 0f, _settings.InCurve.Evaluate(distance / halfWay));
                }
                else
                {
                    newPosition.y = Mathf.Lerp(0f, _settings.JumpHeight, _settings.OutCurve.Evaluate(distance / halfWay));
                }
            }

            transform.position = new Vector3(Mathf.Clamp(newPosition.x, -_settings.XPositionCap, _settings.XPositionCap),
                Mathf.Clamp(newPosition.y, 0f, _settings.JumpHeight), newPosition.z);
        }

        private float GetHalfWay(PathLine nextPathLine)
        {
            return (nextPathLine.Position.z - _currentPathLine.Position.z) / 2f;
        }

        private PathLine GetNextPathLine()
        {
            return _path.GetNextPathLine(_currentPathLine);
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            var pathLine = otherCollider.GetComponent<PathLine>();
            if (pathLine != null)
            {
                _currentPathLine = pathLine;
            }
            else
            {
                var platform = otherCollider.GetComponent<Platform>();
                if (platform != null)
                {
                    _isOnPlatform = true;
                    
                    var blot = _blotSpawner.Spawn() as Blot;
                    if (blot != null)
                    {
                        blot.Place(transform.position, platform.BaseTransform, _color);
                    }
                    
                    platform.Trigger();
                    GameController.Instance.GameSession.AddScorePoints(1);
                }
                else
                {
                    var fallZone = otherCollider.GetComponent<FallZone>();
                    if (fallZone != null && !_isOnPlatform)
                    {
                        _rigidbody.useGravity = true;
                        _rigidbody.isKinematic = false;
                        _rigidbody.AddForce(Vector3.down * _settings.MoveSpeed, ForceMode.Impulse);
                        SignalsManager.Broadcast(_stateSignal.Name, GameState.End.ToString());
                    }
                }
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

        private void OnDestroy()
        {
            _inputProvider.PointerDown -= OnPointerDown;
            _inputProvider.PointerUp -= OnPointerUp;
            _inputProvider.BeginDrag -= OnBeginDrag;
            _inputProvider.Drag -= OnDrag;
        }
    }
}