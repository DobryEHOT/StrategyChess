using Assets.Scripts.SelectorSpace;
using Game.MapSystems.MOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Units.MODs;
using Game.Singleton;
using Game.MapSystems;

namespace Game.UI
{
    [RequireComponent(typeof(ChankContainerMOD))]
    public class VizorDataChank : VizorData
    {
        private ChankContainerMOD containerMOD;
        private UnitMOD oldAttackMOD;

        public override void Start()
        {
            base.Start();
            containerMOD = GetComponent<ChankContainerMOD>();
        }

        public override void OnSelectItem(SelectorItems selector)
        {
            var units = containerMOD.GetUnitSlots();
            foreach (var u in units)
                if (u.Value is ISelectItem item)
                    item.OnSelectItem(selector);
        }

        public override void OnUnselectItem(SelectorItems selector)
        {
            var units = containerMOD.GetUnitSlots();
            foreach (var u in units)
                if (u.Value is ISelectItem item)
                    item.OnUnselectItem(selector);
        }

        public override void OnAimSelectItem(SelectorItems selector)
        {
            if (containerMOD == null)
                return;

            List<VizorItem> list = GetAndShowHint();
            vizorItems = list;
            if (vizorItems.Count == 0)
                return;

            base.OnAimSelectItem(selector);
        }

        // Имеет Side-effect. Собирает и возвращает "подсказки" и показывает сетку аттаки юнита за у проход.
        private List<VizorItem> GetAndShowHint() 
        {
            var list = new List<VizorItem>();
            var container = containerMOD.GetUnitSlots();
            foreach (var element in container)
            {
                TryActiveMapAttack(element);
                var vizorContainer = element.Value.GetComponent<VizorDataContainer>();
                if (vizorContainer != null)
                    list.AddRange(vizorContainer.VizorItems);
            }

            return list;
        }

        public override void OnUnAimSelectItem(SelectorItems selector)
        {
            if (oldAttackMOD != null)
            {
                Singleton<MapSystem>.MainSingleton.ChanksController.DisableSelectorChanksColor();
                oldAttackMOD = null;
            }

            base.OnUnAimSelectItem(selector);
        }

        private void TryActiveMapAttack(KeyValuePair<int, Units.Unit> element)
        {
            if (TryShowModMap(element.Value.GetComponent<ActionUnitAttackWithMoveMOD>()))
                return;
            if (TryShowModMap(element.Value.GetComponent<ActionUnitDistanceAttackMOD>(), ChankSelector.AttackDistanceShower))
                return;
            if (TryShowModMap(element.Value.GetComponent<ActionUnitAttackWithoutMOD>()))
                return;
            if (TryShowModMap(element.Value.GetComponent<ActionUnitAttackWithDeadMOD>()))
                return;
        }

        private bool TryShowModMap(UnitMOD attack, ChankSelector chankSelector = ChankSelector.AttackShower)
        {
            if (attack != null)
            {
                if (oldAttackMOD == null || (oldAttackMOD != null && !oldAttackMOD.Equals(attack)))
                {
                    foreach (var chank in attack.GetListChanks())
                        Singleton<MapSystem>.MainSingleton.ChanksController.ActiveSelector(chank, chankSelector);

                    oldAttackMOD = attack;
                    return true;
                }
            }

            return false;
        }
    }
}
