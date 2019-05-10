using Framework.Input;
using Framework.Signals;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class EndPage : Page<PageModel>
    {
        [SerializeField] private Signal _stateChangeSignal;

        public override void OnEnter()
        {
            base.OnEnter();
            InputEventProvider.Instance.PointerDown += OnPointerPown;
        }

        private void OnPointerPown(PointerEventData eventData)
        {
            SignalsManager.Broadcast(_stateChangeSignal.Name, GameState.Start.ToString());
        }

        public override void OnExit()
        {
            base.OnExit();
            InputEventProvider.Instance.PointerDown -= OnPointerPown;
        }
    }
}