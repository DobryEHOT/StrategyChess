using Assets.Scripts.UnitsSystem.Interfaces;
using Game.CardSystem.MOD;
using Game.MapSystems;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    public class ActionUnitDistanceAttackMOD : ActionAttackMOD, IRaundAction
    {
        public override bool UseAdditialChekerPathTrace => false;
        public string NameMOD = "OnlyAttack";
        [SerializeField] private int intervalAttack = 6;
        [SerializeField] private List<string> cantAttackUnits = new List<string>();
        private int nowCharge = 6;
        
        public override bool isInteractable(Chank chank)
        {
            if (nowCharge < intervalAttack)
                return false;

            if (chank.HeightLvl > RootUnit.StandChank.HeightLvl)
                return false;

            var container = chank.ChankContainer;

            Dictionary<int, Unit> units = null;
            if (container != null)
                units = container.GetUnitSlots();

            foreach (var u in units)
                if (cantAttackUnits.Contains(u.Value.nameUnit))
                    return false;

            Unit unit;
            if (units != null && units.TryGetValue(RootUnit.ChankQueue, out unit))
                if (RootUnit.senor.Team != unit.senor.Team)
                    if (!cantAttackUnits.Contains(unit.nameUnit))
                        return true;

            return false;
        }
        public void OnStartRaund()
        {
            if (nowCharge < intervalAttack)
                nowCharge++;
        }

        public void OnEndRaund()
        {

        }
        protected override void InitUnitMOD()
        {
            ModsName = NameMOD;
        }

        public override void OnMove(Chank chank)
        {
            chank.ClearPawn(RootUnit.ChankQueue);

            var raund = GetComponent<RaundTickerUnitMOD>();
            if (raund != null)
                raund.DoMoveMOD();

            nowCharge = 0;
            base.OnMove(chank);
        }
        public override List<Chank> GetListChanks() => Singleton<MapSystem>.MainSingleton.ChanksController.GetDistanceAttackChanks(this); // ChanksMoving
    }
}