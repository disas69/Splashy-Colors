using System.Collections.Generic;
using Framework.Signals;
using Game.Data;
using Game.Main;
using Game.Spawn;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Spawner))]
    public class LivesViewController : MonoBehaviour
    {
        private List<SpawnableObject> _activeLives;
        private Spawner _livesViewSpawner;
        
        [SerializeField] private Signal _livesSignal;
        [SerializeField] private Signal _colorSignal;

        private void Awake()
        {
            _activeLives = new List<SpawnableObject>();
            _livesViewSpawner = GetComponent<Spawner>();
        }

        public void OnEnter()
        {
            UpdateLives(GameController.Instance.GameSession.Lives);
            
            SignalsManager.Register(_livesSignal.Name, UpdateLives);
            SignalsManager.Register(_colorSignal.Name, ApplyColor);
        }

        private void UpdateLives(int livesCount)
        {
            for (var i = 0; i < _activeLives.Count; i++)
            {
                _activeLives[i].Deactivate();
            }
            
            _activeLives.Clear();

            for (var i = 0; i < livesCount; i++)
            {
                var live = _livesViewSpawner.Spawn();
                if (live != null)
                {
                    live.transform.localScale = Vector3.one;
                    _activeLives.Add(live);
                }
            }
            
            ApplyColor(GameController.Instance.GameSession.Color);
        }

        private void ApplyColor(string color)
        {
            var newColor = GameConfiguration.GetColor(color);
            
            for (var i = 0; i < _activeLives.Count; i++)
            {
                var image = _activeLives[i].GetComponent<Image>();
                if (image != null)
                {
                    image.color = newColor;
                }
            }
        }

        public void OnExit()
        {
            SignalsManager.Unregister(_livesSignal.Name, UpdateLives);
            SignalsManager.Unregister(_colorSignal.Name, ApplyColor);
        }
    }
}