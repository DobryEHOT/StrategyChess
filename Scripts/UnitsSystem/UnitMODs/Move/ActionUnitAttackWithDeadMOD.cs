using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    public class ActionUnitAttackWithDeadMOD : ActionUnitAttackWithMoveMOD
    {
        [SerializeField]
        private List<string> deadOnAttackUnits = new List<string>();

        public override void OnMove(Chank chank)
        {
            var container = chank.ChankContainer;
            if (container == null)
                return;
 
            Unit unit;
            bool isDead = false;
            if (container.TryGetUnit(RootUnit.ChankQueue, out unit))
                if (deadOnAttackUnits.Contains(unit.nameUnit))
                    isDead = true;

            base.OnMove(chank);
            if (isDead)
                RootUnit.Dead();

        }
    }
}