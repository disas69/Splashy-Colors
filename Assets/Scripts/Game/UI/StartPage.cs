﻿using System;
using System.Collections;
using Framework.Extensions;
using Framework.Input;
using Framework.Localization;
using Framework.Signals;
using Framework.UI.Structure.Base.Model;
using Framework.UI.Structure.Base.View;
using Game.Data;
using Game.Main;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class StartPage : Page<PageModel>
    {
        private Coroutine _overlayTransitionCoroutine;

        [SerializeField] private RectTransform _header;
        [SerializeField] private CanvasGroup _overlay;
        [SerializeField] private float _overlayTransitionSpeed;
        [SerializeField] private Signal _stateChangeSignal;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _bestScore;

        public override void OnEnter()
        {
            base.OnEnter();
            InputEventProvider.Instance.PointerDown += OnPointerDown;

            if (GameData.Data.BestScore > 0)
            {
                _header.gameObject.SetActive(true);
                _bestScore.text = GameData.Data.BestScore.ToString();
            }
            else
            {
                _header.gameObject.SetActive(false);
            }

            _level.text = string.Format(LocalizationManager.GetString("Level"), GameData.Data.Level);
        }

        protected override IEnumerator InTransition(Action callback)
        {
            _overlayTransitionCoroutine = StartCoroutine(ShowOverlay());
            yield return _overlayTransitionCoroutine;
        }

        private IEnumerator ShowOverlay()
        {
            _overlay.gameObject.SetActive(true);
            _overlay.alpha = 1f;

            while (_overlay.alpha > 0f)
            {
                _overlay.alpha -= _overlayTransitionSpeed * 2f * Time.deltaTime;
                yield return null;
            }

            _overlay.alpha = 0f;
            _overlay.gameObject.SetActive(false);
            _overlayTransitionCoroutine = null;
        }

        private void OnPointerDown(PointerEventData eventData)
        {
            SignalsManager.Broadcast(_stateChangeSignal.Name, GameState.Play.ToString());
        }

        public override void OnExit()
        {
            _overlay.gameObject.SetActive(false);
            this.SafeStopCoroutine(_overlayTransitionCoroutine);
            InputEventProvider.Instance.PointerDown -= OnPointerDown;
            base.OnExit();
        }
    }
}