using Game.Data;
using Game.Spawn;
using UnityEngine;

namespace Game.Objects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Blot : SpawnableObject
    {
        private SpriteRenderer _renderer;

        [SerializeField] private Vector3 _defaultPosition;
        [SerializeField] private Vector3 _defaultRotation;
        [SerializeField] private Vector3 _defaultScale;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Material _splashMaterial;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void Place(Vector3 position, Transform parent, string color)
        {
            var newPosition = _defaultPosition;
            newPosition.x = position.x;
            transform.position = newPosition;
            transform.localEulerAngles = _defaultRotation;
            transform.localScale = _defaultScale;
            transform.SetParent(parent, true);

            ApplyColor(color);
            Randomize();

            _particleSystem.Play();
        }

        public void ApplyColor(string color)
        {
            _renderer.color = GameConfiguration.GetColor(color);
            _splashMaterial.color = GameConfiguration.GetColor(color);
        }

        private void Randomize()
        {
            _renderer.flipX = Random.value > 0.5f;
            _renderer.flipY = Random.value > 0.5f;
        }
    }
}