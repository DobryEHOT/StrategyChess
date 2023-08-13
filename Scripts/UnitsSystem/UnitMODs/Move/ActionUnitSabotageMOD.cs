using Assets.Scripts.UnitsSystem.Interfaces;
using Game.CardSystem.MOD;
using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    public class ActionUnitSabotageMOD : UnitMOD
    {
        public string NameMOD = "Sabotage";

        [SerializeField] private int QueueUnitSabotage = 1;
        [SerializeField] private List<string> cantAttackUnits = new List<string>();

        public override bool isInteractable(Chank chank)
        {
            Unit unit;
            if (TryGetUnitForSabotage(chank, out unit))
                if (!cantAttackUnits.Contains(unit.nameUnit))
                    return true;

            return false;
        }

        private bool TryGetUnitForSabotage(Chank chank, out Unit unit)
        {
            var container = chank.ChankContainer;
            unit = null;

            if (container == null)
                return false;

            if (container.IsFree(RootUnit.ChankQueue) && container.TryGetUnit(QueueUnitSabotage, out unit))
                return true;

            return false;
        }

        public override void OnMove(Chank chank)
        {
            chank.ClearPawn(QueueUnitSabotage);
            chank.SetActiveMeshRenderChank(true);

            var raund = GetComponent<RaundTickerUnitMOD>();
            if (raund != null)
                raund.DoMoveMOD();

            base.OnMove(chank);
        }

        protected override void InitUnitMOD()
        {
            ModsName = NameMOD;
        }

    }
}
