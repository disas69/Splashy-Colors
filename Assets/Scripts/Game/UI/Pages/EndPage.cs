using Framework.Extensions;
using Framework.Signals;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Main;
using UnityEngine;

namespace Game.UI.Pages
{
    public class EndPage : Page<PageModel>
    {
        [SerializeField] private float _waitTime;
        [SerializeField] private Signal _stateChangeSignal;

        public override void OnEnter()
        {
            base.OnEnter();
            this.WaitForSeconds(_waitTime, () =>
            {
                SignalsManager.Broadcast(_stateChangeSignal.Name, GameState.Start.ToString());
            });
        }
    }
}