using Framework.Signals;
using Game.Data;
using Game.Objects;
using Game.Path;
using UnityEngine;

namespace Game.Main
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private Ball _ball;
        [SerializeField] private PathController _path;
        [SerializeField] private Signal _levelSignal;
        [SerializeField] private Signal _scoreSignal;
        [SerializeField] private Signal _livesSignal;
        [SerializeField] private Signal _colorSignal;

        public int Lives { get; private set; }
        public int Level { get; private set; }
        public int Score { get; private set; }
        public int ScoreMultiplier { get; private set; }
        public string Color { get; private set; }

        public void ResetSession()
        {
            ApplyColor(GameConfiguration.GetRandomColorName());
            Level = GameData.Data.Level;
            
            _ball.ResetBall();
            _path.ResetPath();
        }

        public void StartSession()
        {
            Lives = GameConfiguration.Instance.Lives;
            Score = 0;
            ScoreMultiplier = 1;

            _ball.Activate();
            _path.Activate();
        }

        public void StopSession()
        {
            _ball.Deactivate();
            _path.Deactivate();

            GameData.Data.CurrentScore = Score;

            if (Score > GameData.Data.BestScore)
            {
                GameData.Data.Level = Level;
                GameData.Data.BestScore = Score;
                GameData.Save();
            }
        }

        public void SubtractLive(bool all = false)
        {
            if (all)
            {
                Lives = 0;
            }
            else
            {
                Lives--;
            }

            if (Lives > 0)
            {
                SignalsManager.Broadcast(_livesSignal.Name, Lives);
            }
            else
            {
                _ball.BlowUp();
                GameController.Instance.SetState(GameState.End);
            }
        }

        public void AddScorePoints(int scorePoints)
        {
            Score += scorePoints;

            var level = GameConfiguration.GetLevelByScore(Score);
            if (level > Level)
            {
                Level = level;
                SignalsManager.Broadcast(_levelSignal.Name, level);
            }
            
            SignalsManager.Broadcast(_scoreSignal.Name, Score);
        }

        public void ApplyColor(string color)
        {
            if (color != Color)
            {
                Color = color;
                _ball.ApplyColor(color);
                _path.ApplyColor(color);
                SignalsManager.Broadcast(_colorSignal.Name, color);
            }
            else
            {
                Lives++;
                SignalsManager.Broadcast(_livesSignal.Name, Lives);
            }
        }

        public void ApplyMultiplier(int multiplier)
        {
            ScoreMultiplier *= multiplier;
        }
    }
}