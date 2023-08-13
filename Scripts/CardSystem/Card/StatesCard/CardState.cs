using Game.MapSystems.Enums;

namespace Game.CardSystem.StatesCard
{
    public abstract class CardState
    {
        protected Player player;
        protected GameTeam lochalPlayerTeam;

        public CardState(Player player)
        {
            this.player = player;
            lochalPlayerTeam = LochalClientInformation.MainSingleton.Info.Team;

        }

        public abstract void DoAction(CardMover mover, StatesManipulator manipulator);
        public abstract void ExitState(CardMover mover, StatesManipulator manipulator);
        public abstract void EnterState(CardMover mover, StatesManipulator manipulator);
    }
}
