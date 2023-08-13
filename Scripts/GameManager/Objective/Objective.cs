using Assets.Scripts.GameManager.States;
using Game.MapSystems.Enums;
using Game.Units;

namespace Game
{
    public abstract class Objective
    {
        public GameTeam WinTeam { get; private set; } = GameTeam.Neutral;

        protected bool isClose = false;
        protected GameStatesController gameStatesController;

        public void SetStatesController(GameStatesController statesController) => gameStatesController = statesController;
       
        public abstract void Check(Unit deadUnit);

        public virtual void Reset()
        {
            isClose = false;
            WinTeam = GameTeam.Neutral;
        }

        protected void SetWinTeam(GameTeam team)
        {
            if (isClose)
                return;

            isClose = true;
            WinTeam = team;

            if (gameStatesController != null)
                gameStatesController.EndGame();
        }
    }
}
