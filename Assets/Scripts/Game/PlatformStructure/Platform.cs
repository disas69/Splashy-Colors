using Game.PathStructure;
using Game.SpawnStructure;
using UnityEngine;

namespace Game.PlatformStructure
{
    [RequireComponent(typeof(BoxCollider), typeof(Animator))]
    public class Platform : SpawnableObject
    {
        private PathLine _pathLine;
        private BoxCollider _collider;
        private Animator _animator;

        public PathLine PathLine
        {
            get { return _pathLine; }
        }

        private void Awake()
        {
            _pathLine = GetComponentInParent<PathLine>();
            _collider = GetComponent<BoxCollider>();
            _animator = GetComponent<Animator>();
        }

        public void Trigger()
        {
            if (GameController.Instance.GameState == GameState.Play)
            {
                _animator.SetTrigger("React");
            }
        }
    }
}