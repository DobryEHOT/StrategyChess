using System;
using System.Collections.Generic;

namespace Game.CardSystem.StatesCard
{
    public class StatesManipulator
    {
        private List<CardState> states = new List<CardState>();
        private CardMover cardMover;
        private CardState activeState;
        public StatesManipulator(CardMover rootCard, Player player)
        {
            cardMover = rootCard;
            states.Add(new IdleCardState(player));
            states.Add(new SelectCardState(player));
            states.Add(new KeepCardState(player));
            activeState = FindState<IdleCardState>();
            activeState.EnterState(cardMover, this);
        }

        public void CloseManipulator() => activeState.ExitState(cardMover, this);

        public void DoSelected()
        {
            if (activeState is IdleCardState)
                SwichTo<SelectCardState>();
        }

        public void DoUpdate()
        {
            activeState.DoAction(cardMover, this);
        }

        public void SwichTo<T>() where T : CardState
        {
            if (activeState is T)
                return;

            var nextState = FindState<T>();
            activeState.ExitState(cardMover, this);
            nextState.EnterState(cardMover, this);
            activeState = nextState;
        }

        private CardState FindState<T>() where T : CardState
        {
            foreach (var state in states)
                if (state is T)
                    return state;

            throw new Exception($"StateManipulator dosen't have state {typeof(T).Name}");
        }
    }
}
