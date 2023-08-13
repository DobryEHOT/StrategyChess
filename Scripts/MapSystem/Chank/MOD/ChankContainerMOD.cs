using Game.CardSystem;
using Game.MapSystems.Enums;
using Game.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapSystems.MOD
{
    public class ChankContainerMOD : ChankMOD
    {
        private Dictionary<int, Unit> UnitSlots { get; set; } = new Dictionary<int, Unit>();
        public bool firstIsUnit = false;
        public Unit unitStay;

        [SerializeField]
        private bool alwaysBusy = false;

        private void FixedUpdate()
        {
            //if (UnitSlots.ContainsKey(0)) //Лютый костыль, надо будет пофиксить
            //{
            //    firstIsUnit = true;
            //    unitStay = UnitSlots[0];
            //    CheckerCliner(0);
            //}
            //else
            //{
            //    firstIsUnit = false;
            //    unitStay = null;
            //}
        }

        public bool ContainUnit(string nameUnit) 
        {
            foreach (var un in UnitSlots)
                if (un.Value.nameUnit.Equals(nameUnit))
                    return true;

            return false;
        }

        public List<Unit> GetTeamUnits(GameTeam team)
        {
            List<Unit> units = new List<Unit>();
            if (UnitSlots.Count == 0)
                return units;

            foreach (var unit in UnitSlots)
                if (unit.Value.senor.Team == team)
                    units.Add(unit.Value);

            return units;
        }

        public List<Unit> GetUnteamUnits(GameTeam team)
        {
            List<Unit> units = new List<Unit>();
            if (UnitSlots.Count == 0)
                return units;

            foreach (var unit in UnitSlots)
                if (unit.Value.senor.Team != team)
                    units.Add(unit.Value);

            return units;
        }


        public Dictionary<int, Unit> GetUnitSlots() => UnitSlots;

        public void SetAlwaysBusy(bool setter) => alwaysBusy = setter;

        public bool TryGetUnit(int numberQueue, out Unit unit)
        {
            if (UnitSlots.TryGetValue(numberQueue, out unit))
                return true;
            return false;
        }

        public bool TrySetUnit(Unit unitSlot)
        {
            if(unitSlot.nameUnit == "Castle_House")
            {

            }

            if (unitSlot == null || !IsFree(unitSlot))
                return false;

            UnitSlots.Add(unitSlot.ChankQueue, unitSlot);
            return true;
        }

        public Unit ClearUnitSlot(int numberQueue)
        {
            //if (IsFree(numberQueue))
            //    return null;

            Unit unit = null;
            if (UnitSlots.TryGetValue(numberQueue, out unit))
            {
                UnitSlots.Remove(numberQueue);
                return unit;
            }

            UnitSlots.Remove(numberQueue);
            return null;
        }

        public void ClearAllUnitSlots()
        {
            UnitSlots.Clear();
        }


        public void ClearUnitSlotWithDestroy(int numberQueue)
        {
            //if (IsFree(numberQueue))
            //    return;

            Unit unit = null;
            if (UnitSlots.TryGetValue(numberQueue, out unit))
            {
                if (unit != null)
                    Destroy(unit.gameObject);
            }

            UnitSlots.Remove(numberQueue);
        }

        public bool IsFree(int numberQueue)
        {
            CheckerCliner(numberQueue);

            if (alwaysBusy)
                return false;

            if (!UnitSlots.ContainsKey(numberQueue))
                return true;

            return false;
        }

        private void CheckerCliner(int numberQueue)
        {
            if (UnitSlots.ContainsKey(numberQueue) && UnitSlots[numberQueue] == null)
                UnitSlots.Remove(numberQueue);
        }

        public bool IsFree(Card fromCard)
        {
            if (alwaysBusy)
                return false;

            if (fromCard == null)
                return false;

            CheckerCliner(fromCard.GetUnit().ChankQueue);
            if (!UnitSlots.ContainsKey(fromCard.GetUnit().ChankQueue))
                return true;

            return false;
        }

        public bool IsFree(Unit fromUnit)
        {
            if (alwaysBusy)
                return false;

            if (fromUnit == null)
                return false;

            CheckerCliner(fromUnit.ChankQueue);
            if (!UnitSlots.ContainsKey(fromUnit.ChankQueue))
                return true;

            return false;
        }

    }
}