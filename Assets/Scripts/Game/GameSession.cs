using Game.BallStructure;
using Game.PathStructure;
using UnityEngine;

namespace Game
{
    public class GameSession : MonoBehaviour
    {
        private int _lives;
        private int _score;

        [SerializeField] private Ball _ball;
        [SerializeField] private Path _path;

        public void ResetSession()
        {
            _ball.ResetBall();
            _path.ResetPath();
        }

        public void StartSession()
        {
            _lives = 0;
            _score = 0;
            StartGame();
        }

        public void StopSession()
        {
            StopGame();
            GameData.Data.CurrentScore = _score;

            if (_score > GameData.Data.BestScore)
            {
                GameData.Data.BestScore = _score;
                GameData.Save();
            }
        }

        public void OnShipDestroyed()
        {
            _lives--;

            if (_lives > 0)
            {
                //SignalsManager.Broadcast(_livesSignal.Name, _lives);
            }
            else
            {
                GameController.Instance.SetState(GameState.End);
            }
        }

        public void OnAsteroidDestroyed(int scorePoints)
        {
            _score += scorePoints;
            //SignalsManager.Broadcast(_scoreSignal.Name, _score);
        }

        private void StartGame()
        {
            _ball.Activate();
            _path.Activate();
        }

        private void StopGame()
        {
            _ball.Deactivate();
            _path.Deactivate();
        }
    }
}