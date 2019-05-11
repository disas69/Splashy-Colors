using Framework.Signals;
using Game.Colors;
using Game.Main;
using Game.Spawn;
using UnityEngine;

namespace Game.Objects
{
    [RequireComponent(typeof(BoxCollider), typeof(Animator))]
    public class Platform : SpawnableObject
    {
        private Animator _animator;
        private string _color;

        [SerializeField] private PlatformSettings _settings;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Signal _audioSignal;

        public Transform BaseTransform => _renderer.gameObject.transform;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Trigger()
        {
            if (GameController.Instance.GameState == GameState.Play)
            {
                _animator.SetTrigger("React");
                SignalsManager.Broadcast(_audioSignal.Name, "tap");
            }
        }

        public void ApplyColor(string color)
        {
            _color = color;
            _renderer.sharedMaterial = ColorSettings.GetMaterial(color);
        }

        public override void Deactivate()
        {
            var blots = GetComponentsInChildren<Blot>();

            for (var i = 0; i < blots.Length; i++)
            {
                blots[i].Deactivate();
            }
            
            base.Deactivate();
        }
    }
}