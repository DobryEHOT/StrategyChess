using Game.MapSystems;
using UnityEngine;

namespace Game.Units.MODs
{
    public class ActionUnitAttackWithMoveMOD : ActionAttackMOD
    {
        public string NameMOD = "MoveAttack";

        public override bool UseAdditialChekerPathTrace { get; } = false;

        [SerializeField] private string soundAction = "Punch";

        public override bool isInteractable(Chank chank)
        {
            var container = chank.ChankContainer;

            if (container == null)
                return false;

            Unit unit;
            if (container.TryGetUnit(RootUnit.ChankQueue, out unit))
                if (RootUnit.senor.Team != unit.senor.Team)
                    return true;

            return false;
        }

        public override void OnMove(Chank chank)
        {

            chank.ClearPawn(RootUnit.ChankQueue);
            var pos = RootUnit.GetDefaultPositionToChank(chank);
            RootUnit.transform.position = pos;
            RootUnit.SwitchStandChank(chank);

            var raund = GetComponent<RaundTickerUnitMOD>();
            if (raund != null)
                raund.DoMoveMOD();

            base.OnMove(chank);
        }

        protected override void InitUnitMOD()
        {
            nameSound = soundAction;
            ModsName = NameMOD;
        }

    }
}
