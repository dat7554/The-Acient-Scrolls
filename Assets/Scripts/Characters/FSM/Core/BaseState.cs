using System;

namespace Characters.FSM.Core
{
    public abstract class BaseState<T> where T : Enum
    {
        public T StateID { get; private set; }

        protected BaseState(T stateID)
        {
            StateID = stateID;
        }
        
        /// <summary>
        /// Code that runs when first enter the state
        /// </summary>
        public abstract void EnterState();
    
        /// <summary>
        /// Per-frame logic, include condition to transition to a new state
        /// </summary>
        public abstract void ExecuteState();
        
        /// <summary>
        /// Code that runs when exit the state
        /// </summary>
        public abstract void ExitState();
        
        public abstract T GetNextState();
    }
}