using Framework.Signals;
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
        [SerializeField] private Signal _audioSignal;
        [SerializeField] private string _sound;

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
            
            SignalsManager.Broadcast(_audioSignal.Name, _sound);
            GameController.Instance.GameSession.ApplyMultiplier(_multiplier);
            Debug.Log("Trigger Multiplier");
        }
    }
}