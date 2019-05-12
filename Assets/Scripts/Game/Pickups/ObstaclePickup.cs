using Game.Main;
using UnityEngine;

namespace Game.Pickups
{
    public class ObstaclePickup : Pickup
    {
        public override PickupType Type => PickupType.Obstacle;

        public override void Trigger()
        {
            base.Trigger();
            GameController.Instance.GameSession.SubtractLive(true);
            Debug.Log("Trigger Obstacle");
        }
    }
}