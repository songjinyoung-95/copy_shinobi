using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Meow_Moew_Shinobi.FSM
{
    public interface IState<T>
    {
        void Enter();
        void Execute();
        void Exit();
        void FixedUpdate();
        void Update();
    }

    public class FiniteStateMachine<T>
    {
        public IState<T> CurrentState => _currentState;

        private T _owner;
        private IState<T> _currentState;
        private IState<T> _previousState;

        public void Log()
        {
            Debug.Log($"{_currentState} {_previousState}");
        }

        public FiniteStateMachine(T owner)
        {
            _owner          = owner;
            _currentState   = null;
            _previousState  = null;
        }

        public void ChangeState(IState<T> newState)
        {
            if(_currentState != null)
                _currentState.Exit();

            if(_currentState == newState)
                return;

            _previousState = _currentState;
            _currentState  = newState;
            
            _currentState.Enter();
        }

        public bool TryRevertToPreviousState()
        {
            if(_previousState == null)
                return false;

            ChangeState(_previousState);
            return true;
        }
    }
}