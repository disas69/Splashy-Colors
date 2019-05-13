using Game.Data;
using Game.Main;
using TMPro;
using UnityEngine;

namespace Game.Pickups
{
    public class MultiplierPickup : Pickup
    {
        private int _multiplier;

        [SerializeField] private TextMeshPro _text;

        public override void Activate()
        {
            var level = GameConfiguration.GetLevelSettings(GameController.Instance.GameSession.Level);
            if (level != null)
            {
                _multiplier = level.LineSettings.ScoreMultiplier;
                _text.text = string.Format("x{0}", _multiplier);
            }
        }

        public override void Trigger()
        {
            base.Trigger();
            GameController.Instance.GameSession.ApplyMultiplier(_multiplier);
            Debug.Log("Trigger Multiplier");
        }
    }
}