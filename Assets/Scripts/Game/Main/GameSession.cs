using Framework.Signals;
using Game.Objects;
using Game.Path;
using UnityEngine;

namespace Game.Main
{
    public class GameSession : MonoBehaviour
    {
        private int _lives;
        private int _score;
        private string _color;

        [SerializeField] private Ball _ball;
        [SerializeField] private PathController _path;
        [SerializeField] private Signal _scoreSignal;
        [SerializeField] private Signal _livesSignal;
        [SerializeField] private Signal _colorSignal;

        public int Lives => _lives;
        public int Score => _score;
        public string Color => _color;

        public void ResetSession()
        {
            ApplyColor(GameConfiguration.GetRandomColorName());
            
            _ball.ResetBall();
            _path.ResetPath();
        }

        public void StartSession()
        {
            _lives = GameConfiguration.Instance.Lives;
            _score = 0;

            _ball.Activate();
            _path.Activate();
        }

        public void StopSession()
        {
            _ball.Deactivate();
            _path.Deactivate();

            GameData.Data.CurrentScore = _score;

            if (_score > GameData.Data.BestScore)
            {
                GameData.Data.BestScore = _score;
                GameData.Save();
            }
        }

        public void SubtractLive()
        {
            _lives--;

            if (_lives > 0)
            {
                SignalsManager.Broadcast(_livesSignal.Name, _lives);
            }
            else
            {
                _ball.BlowUp();
                GameController.Instance.SetState(GameState.End);
            }
        }

        public void AddScorePoints(int scorePoints)
        {
            _score += scorePoints;
            SignalsManager.Broadcast(_scoreSignal.Name, _score);
        }

        public void ApplyColor(string color)
        {
            _color = color;
            _ball.ApplyColor(_color);
            _path.ApplyColor(_color);
            SignalsManager.Broadcast(_colorSignal.Name, _color);
        }
    }
}