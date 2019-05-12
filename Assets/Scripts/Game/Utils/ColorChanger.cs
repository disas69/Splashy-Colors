using System.Collections;
using UnityEngine;

namespace Game.Utils
{
    public class ColorChanger : MonoBehaviour
    {
        private float _time;
        private Coroutine _colorCoroutine;

        [SerializeField] private MeshRenderer _renderer;

        public void ChangeColor(Material target, float time)
        {
            if (_colorCoroutine != null)
            {
                StopCoroutine(_colorCoroutine);
            }

            _time = 0f;
            _colorCoroutine = StartCoroutine(ChangeColorCoroutine(_renderer.material, target, time));
        }
        
        private IEnumerator ChangeColorCoroutine(Material current, Material target, float time)
        {
            while (_time < time)
            {
                current.Lerp(current, target, _time/time);
                _time += Time.deltaTime;
                yield return null;
            }

            _renderer.sharedMaterial = target;
        }
    }
}