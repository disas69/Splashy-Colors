using Framework.Signals;
using Game.Data;
using Game.Main;
using UnityEngine;

namespace Game.Pickups
{
    public class ColorPickup : Pickup
    {
        private string _color;

        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Material _splashMaterial;
        [SerializeField] private Signal _audioSignal;
        [SerializeField] private string _sound;

        public override void Activate()
        {
            _renderer.enabled = true;
            _color = GameConfiguration.GetRandomColorName();
            _renderer.sharedMaterial = GameConfiguration.GetMaterial(_color);
        }

        public override void Trigger()
        {
            _renderer.enabled = false;
            _splashMaterial.color = GameConfiguration.GetColor(_color);
            _particleSystem.Play();
            SignalsManager.Broadcast(_audioSignal.Name, _sound);
            GameController.Instance.GameSession.ApplyColor(_color);
            Debug.Log("Trigger Color");
        }
    }
}