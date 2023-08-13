namespace Assets.Scripts.StateMachin
{
    public abstract class State
    {
        public abstract void Enter(StateMachine machine);
        public abstract void Update(StateMachine machine);
        public abstract void Exit(StateMachine machine);
    }

    public class NoneState : State
    {
        public override void Enter(StateMachine machine) { }

        public override void Exit(StateMachine machine) { }

        public override void Update(StateMachine machine) { }
    }
}
