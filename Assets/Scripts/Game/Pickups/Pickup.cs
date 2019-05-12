using Game.Spawn;
using UnityEngine;

namespace Game.Pickups
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Pickup : SpawnableObject
    {
        [SerializeField] private Vector3 _defaultPosition;
        [SerializeField] private Vector3 _defaultRotation;
        [SerializeField] private Vector3 _defaultScale;

        public abstract PickupType Type { get; }

        public void Place(Vector3 position, Transform parent)
        {
            var newPosition = _defaultPosition;
            newPosition.x = position.x;
            newPosition.z = position.z;
            transform.position = newPosition;
            transform.localEulerAngles = _defaultRotation;
            transform.localScale = _defaultScale;
            transform.SetParent(parent, true);
        }

        public virtual void Activate()
        {
        }

        public virtual void Trigger()
        {
            Deactivate();
        }
    }
}