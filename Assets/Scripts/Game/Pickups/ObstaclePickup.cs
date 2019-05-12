using Framework.Signals;
using Game.Data;
using Game.Main;
using Game.Objects;
using UnityEngine;

namespace Game.Pickups
{
    public class ObstaclePickup : Pickup
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Material _splashMaterial;
        [SerializeField] private Signal _colorSignal;
        
        public override PickupType Type => PickupType.Obstacle;

        public override void Activate()
        {
            base.Activate();

            UpdateColor(GameController.Instance.GameSession.Color);
            SignalsManager.Register(_colorSignal.Name, UpdateColor);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            SignalsManager.Unregister(_colorSignal.Name, UpdateColor);
        }

        public override void Trigger()
        {
            _particleSystem.Play();
            GameController.Instance.GameSession.SubtractLive(true);
            Debug.Log("Trigger Obstacle");
        }

        private void UpdateColor(string color)
        {
            var platform = GetComponentInParent<Platform>();
            if (platform != null)
            {
                var material = GameConfiguration.GetMaterial(platform.Color);
                var materials = new Material[_renderer.materials.Length];
                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }

                _renderer.materials = materials;
            }
            
            _splashMaterial.color = GameConfiguration.GetColor(color);
        }
    }
}