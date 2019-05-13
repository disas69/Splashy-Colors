using Framework.Localization;
using Framework.Signals;
using Framework.UI.Notifications;
using Framework.UI.Notifications.Model;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Main;
using Game.Utils;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PlayPage : Page<PageModel>
    {
        [SerializeField] private LivesViewController _livesViewController;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private Signal _levelSignal;
        [SerializeField] private Signal _scoreSignal;

        public override void OnEnter()
        {
            base.OnEnter();

            _livesViewController.Subscribe();
            _livesViewController.UpdateLives(GameController.Instance.GameSession.Lives);
            
            _level.text = string.Format(LocalizationManager.GetString("Level"), GameController.Instance.GameSession.Level);
            _score.text = GameController.Instance.GameSession.Score.ToString();

            SignalsManager.Register(_levelSignal.Name, OnLevelChange);
            SignalsManager.Register(_scoreSignal.Name, OnScoreChanged);
        }

        private void OnLevelChange(int level)
        {
            _level.text = string.Format(LocalizationManager.GetString("Level"), level);
            NotificationManager.Show(new TextNotification(LocalizationManager.GetString("NewLevel")), 2f);
        }

        private void OnScoreChanged(int score)
        {
            _score.text = score.ToString();
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