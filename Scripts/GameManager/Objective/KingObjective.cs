using Game.MapSystems.Enums;
using Game.Units;

namespace Game
{
    public class KingObjective : Objective
    {
        private string kingUnitName = "King";

        public override void Check(Unit deadUnit)
        {
            if (!kingUnitName.Equals(deadUnit.nameUnit))
                return;

            var looseTeam = deadUnit.senor.Team;
            if (looseTeam == GameTeam.Blue)
                SetWinTeam(GameTeam.Red);
            if (looseTeam == GameTeam.Red)
                SetWinTeam(GameTeam.Blue);
        }
    }
}
