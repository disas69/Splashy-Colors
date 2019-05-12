using Framework.Signals;
using Game.Data;
using Game.Main;
using Game.Pickups;
using Game.Spawn;
using Game.Utils;
using UnityEngine;

namespace Game.Objects
{
    [RequireComponent(typeof(BoxCollider), typeof(Animator))]
    public class Platform : SpawnableObject
    {
        private Animator _animator;
        private string _color;

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private ColorChanger _colorChanger;
        [SerializeField] private Signal _audioSignal;
        [SerializeField] private string _soundName;

        public Transform BaseTransform => _renderer.gameObject.transform;
        public string Color => _color;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Trigger()
        {
            if (GameController.Instance.GameState == GameState.Play)
            {
                _animator.SetTrigger("React");
                SignalsManager.Broadcast(_audioSignal.Name, _soundName);
            }
        }

        public void ApplyColor(string color)
        {
            _color = color;
            _colorChanger.ChangeColor(GameConfiguration.GetMaterial(color), GameConfiguration.Instance.ColorChangeTime);
        }

        public override void Deactivate()
        {
            var blots = GetComponentsInChildren<Blot>();

            for (var i = 0; i < blots.Length; i++)
            {
                blots[i].Deactivate();
            }

            var hints = GetComponentsInChildren<Hint>();

            for (var i = 0; i < hints.Length; i++)
            {
                hints[i].Deactivate();
            }

            var pickups = GetComponentsInChildren<Pickup>();

            for (var i = 0; i < pickups.Length; i++)
            {
                pickups[i].Deactivate();
            }
            
            base.Deactivate();
        }
    }
}