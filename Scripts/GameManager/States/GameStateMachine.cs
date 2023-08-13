using Assets.Scripts.StateMachin;

namespace Assets.Scripts.GameManager.States
{
    public class GameStateMachine : StateMachine
    {
        private Game.GameManager manager;
        public GameStateMachine(Game.GameManager gameManager)
        {
            manager = gameManager;
            InicializeMachine();
        }

        protected override void InicializeMachine()
        {
            AddState(new CreateGameState(manager));
            AddState(new StartGameState(manager));
            AddState(new ProcessGameState(manager));
            AddState(new OwnerMoveGameState(manager));
            AddState(new OwnerWaitGameState(manager));
            AddState(new EndGameState(manager));

            SwichTo<CreateGameState>();
        }
    }
}
