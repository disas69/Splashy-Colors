using Framework.Signals;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Main;
using TMPro;
using UnityEngine;

namespace Game.UI.Pages
{
    public class PlayPage : Page<PageModel>
    {
        [SerializeField] private LivesViewController _livesViewController;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private Signal _scoreSignal;

        public override void OnEnter()
        {
            base.OnEnter();

            _livesViewController.OnEnter();
            _score.text = GameController.Instance.GameSession.Score.ToString();
            SignalsManager.Register(_scoreSignal.Name, OnScoreChanged);
        }

        private void OnScoreChanged(int score)
        {
            _score.text = score.ToString();
        }

        public override void OnExit()
        {
            base.OnExit();
            _livesViewController.OnExit();
            SignalsManager.Unregister(_scoreSignal.Name, OnScoreChanged);
        }
    }
}