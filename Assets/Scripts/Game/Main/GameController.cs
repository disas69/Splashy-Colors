﻿using System;
using Framework.Tools.Gameplay;
using Framework.Tools.Singleton;
using Framework.UI;
using Game.Data;
using Game.UI;
using Game.UI.Pages;
using UnityEngine;

namespace Game.Main
{
    [RequireComponent(typeof(GameSession))]
    public class GameController : MonoSingleton<GameController>
    {
        private GameSession _gameSession;
        private StateMachine<GameState> _stateMachine;

        public GameSession GameSession => _gameSession;
        public GameState GameState => _stateMachine.CurrentState;

        protected override void Awake()
        {
            base.Awake();
            _gameSession = GetComponent<GameSession>();
        }

        private void Start()
        {
            GameData.Load();
            CreateStateMachine();
            ActivateStartState();
        }

        public void SetState(string stateName)
        {
            SetState((GameState) Enum.Parse(typeof(GameState), stateName));
        }

        public void SetState(GameState gameState)
        {
            _stateMachine.SetState(gameState);
        }

        private void CreateStateMachine()
        {
            _stateMachine = new StateMachine<GameState>(GameState.Start);
            _stateMachine.AddTransition(GameState.Start, GameState.Play, ActivatePlayState);
            _stateMachine.AddTransition(GameState.Play, GameState.End, ActivateEndState);
            _stateMachine.AddTransition(GameState.End, GameState.Play, ActivatePlayState);
            _stateMachine.AddTransition(GameState.End, GameState.Start, ActivateStartState);
        }

        private void ActivateStartState()
        {
            _gameSession.ResetSession();
            NavigationManager.Instance.OpenScreen<StartPage>();
        }

        private void ActivatePlayState()
        {
            _gameSession.StartSession();
            NavigationManager.Instance.OpenScreen<PlayPage>();
        }

        private void ActivateEndState()
        {
            _gameSession.StopSession();
            NavigationManager.Instance.OpenScreen<EndPage>();
        }

        private void OnDestroy()
        {
            GameData.Save();
        }
    }
}