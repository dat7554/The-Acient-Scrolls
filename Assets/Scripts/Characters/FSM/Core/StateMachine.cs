using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters.FSM.Core
{
    public abstract class StateMachine<T> : MonoBehaviour where T : Enum
    {
        public readonly Dictionary<T, BaseState<T>> States = new ();
        
        public BaseState<T> CurrentState;
        
        private bool _isTransitioningState;

        private void Start()
        {
            CurrentState.EnterState();
        }

        private void Update()
        {
            T nextStateId = CurrentState.GetNextState();

            if (!_isTransitioningState && nextStateId.Equals(CurrentState.StateID))
            {
                CurrentState.ExecuteState();
            }
            else if (!_isTransitioningState)
            {
                TransitionToState(nextStateId);
            }
        }

        public void TransitionToState(T stateId)
        {
            _isTransitioningState = true;
            
            CurrentState.ExitState();
            CurrentState = States[stateId];
            CurrentState.EnterState();
            
            _isTransitioningState = false;
        }
    }
}