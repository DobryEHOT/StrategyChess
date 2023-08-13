using Game.CardSystem.MOD;
using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{

    public class ActionUnitAttackWithoutMOD : ActionUnitAttackWithMoveMOD
    {
        [SerializeField] private List<string> cantAttackUnits = new List<string>();
        public override bool isInteractable(Chank chank)
        {
            var container = chank.ChankContainer;

            if (container == null)
                return false;

            Unit unit;
            if (container.TryGetUnit(RootUnit.ChankQueue, out unit))
                if (RootUnit.senor.Team != unit.senor.Team)
                    if (!cantAttackUnits.Contains(unit.nameUnit))
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

            PlaySound();
        }

        protected override void InitUnitMOD()
        {
            base.InitUnitMOD();
        }

    }
}
