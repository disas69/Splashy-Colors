using Framework.Localization;
using Framework.Signals;
using Framework.UI.Notifications;
using Framework.UI.Notifications.Model;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Data;
using Game.Main;
using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PlayPage : Page<PageModel>
    {
        [SerializeField] private LivesViewController _livesViewController;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private Image _progress;
        [SerializeField] private Signal _levelSignal;
        [SerializeField] private Signal _scoreSignal;
        [SerializeField] private Signal _audioSignal;
        [SerializeField] private string _sound;

        public override void OnEnter()
        {
            base.OnEnter();

            _livesViewController.Subscribe();
            _livesViewController.UpdateLives(GameController.Instance.GameSession.Lives);

            _level.text = string.Format(LocalizationManager.GetString("Level"), GameController.Instance.GameSession.Level);
            _score.text = GameController.Instance.GameSession.Score.ToString();

            UpdateLevelProgress(GameController.Instance.GameSession.Score);
            
            SignalsManager.Register(_levelSignal.Name, OnLevelChange);
            SignalsManager.Register(_scoreSignal.Name, OnScoreChanged);
        }

        private void OnLevelChange(int level)
        {
            _level.text = string.Format(LocalizationManager.GetString("Level"), level);
            SignalsManager.Broadcast(_audioSignal.Name, _sound);
            NotificationManager.Show(new TextNotification(LocalizationManager.GetString("NewLevel")), 2f);
        }

        private void OnScoreChanged(int score)
        {
            _score.text = score.ToString();
            UpdateLevelProgress(score);
        }

        private void UpdateLevelProgress(int score)
        {
            var levelSettings = GameConfiguration.GetLevelSettings(GameController.Instance.GameSession.Level);
            if (levelSettings != null)
            {
                _progress.fillAmount = (float) score / levelSettings.Score;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            _livesViewController.Unsubscribe();

            SignalsManager.Unregister(_levelSignal.Name, OnLevelChange);
            SignalsManager.Unregister(_scoreSignal.Name, OnScoreChanged);
        }
    }
}