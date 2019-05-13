using Framework.Signals;
using Game.Data;
using Game.Main;
using Game.Spawn;
using Game.Utils;
using UnityEngine;

namespace Game.Objects
{
    [RequireComponent(typeof(BoxCollider), typeof(Animator))]
    public class Platform : SpawnableObject
    {
        private Animator _animator;

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private ColorChanger _colorChanger;
        [SerializeField] private Signal _audioSignal;
        [SerializeField] private string _soundName;

        public Transform BaseTransform
        {
            get { return _renderer.gameObject.transform; }
        }

        public string Color { get; private set; }

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
            Color = color;
            _colorChanger.ChangeColor(GameConfiguration.GetMaterial(color), GameConfiguration.Instance.ColorChangeTime);
        }

        public override void Deactivate()
        {
            var spawnableObjects = GetComponentsInChildren<SpawnableObject>();

            for (var i = 0; i < spawnableObjects.Length; i++)
            {
                var spawnableObject = spawnableObjects[i];
                if (spawnableObject != this)
                {
                    spawnableObject.Deactivate();
                }
            }
            
            base.Deactivate();
        }
    }
}