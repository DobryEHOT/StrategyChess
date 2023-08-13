using System;
using System.Collections.Generic;

namespace Assets.Scripts.StateMachin
{
    public abstract class StateMachine
    {
        public State ActiveState { get; private set; }

        private List<State> states = new List<State>();

        public StateMachine()
        {
            states.Add(new NoneState());
            ActiveState = FindState<NoneState>();
        }

        public void CloseManipulator() => ActiveState.Exit(this);

        public void DoUpdate() => ActiveState.Update(this);

        public void SwichTo<T>() where T : State
        {
            if (ActiveState == null)
            {
                var state = FindState<T>();
                ActiveState = state;
                ActiveState.Enter(this);
            }

            if (ActiveState is T)
                return;

            var nextState = FindState<T>();
            ActiveState.Exit(this);
            nextState.Enter(this);
            ActiveState = nextState;
        }

        protected abstract void InicializeMachine();

        protected void AddState(State state) => states.Add(state);

        protected State FindState<T>() where T : State
        {
            foreach (var state in states)
                if (state is T)
                    return state;

            throw new Exception($"StateManipulator dosen't have state {typeof(T).Name}");
        }
    }
}
