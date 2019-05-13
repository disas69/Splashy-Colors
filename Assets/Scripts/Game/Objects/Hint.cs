using Game.Spawn;
using TMPro;
using UnityEngine;

namespace Game.Objects
{
    public class Hint : SpawnableObject
    {
        [SerializeField] private Vector3 _defaultPosition;
        [SerializeField] private Vector3 _defaultRotation;
        [SerializeField] private Vector3 _defaultScale;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private Animation _animation;
        
        public void Place(Vector3 position, Transform parent, int score)
        {
            var newPosition = _defaultPosition;
            newPosition.x = position.x;
            transform.position = newPosition;
            transform.localEulerAngles = _defaultRotation;
            transform.localScale = _defaultScale;
            transform.SetParent(parent, true);

            _text.text = string.Format("+{0}", score);
            _animation.Play();
        }
    }
}