using System.Collections;
using System.Collections.Generic;
using Meow_Moew_Shinobi.Enemy;

namespace Meow_Moew_Shinobi.FSM
{
    public abstract class StateBase<T> : IState<T>
    {
        protected T Owner { get; private set; }

        public StateBase(T owner)
        {
            Owner = owner;
        }

        public abstract void Enter();

        public abstract void Exit();

        public abstract void FixedUpdate();

        public abstract void Execute();

        public abstract void Update();
    }
}