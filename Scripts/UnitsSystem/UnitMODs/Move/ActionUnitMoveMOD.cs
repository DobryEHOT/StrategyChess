using Game.CardSystem.MOD;
using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    public class ActionUnitMoveMOD : UnitMOD
    {
        public string NameMOD = "Move";
        [SerializeField] private string soundAction = "Walk";

        public override bool isInteractable(Chank chank)
        {
            var container = chank.ChankContainer;

            Dictionary<int, Unit> units = null;
            if (container != null)
                units = container.GetUnitSlots();

            Unit unit;
            if (units != null && !units.TryGetValue(RootUnit.ChankQueue, out unit))
                return true;

            return false;
        }

        public override void OnMove(Chank chank)
        {
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