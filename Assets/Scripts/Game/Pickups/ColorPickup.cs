using Game.Data;
using Game.Main;
using UnityEngine;

namespace Game.Pickups
{
    public class ColorPickup : Pickup
    {
        private string _color;

        [SerializeField] private MeshRenderer _renderer;

        public override PickupType Type => PickupType.Color;

        public override void Activate()
        {
            base.Activate();
            
            _color = GameConfiguration.GetRandomColorName();
            _renderer.sharedMaterial = GameConfiguration.GetMaterial(_color);
        }

        public override void Trigger()
        {
            base.Trigger();
            GameController.Instance.GameSession.ApplyColor(_color);
            Debug.Log("Trigger Color");
        }
    }
}