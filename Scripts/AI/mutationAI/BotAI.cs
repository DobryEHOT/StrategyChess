using Assets.Scripts.UnitsSystem.Interfaces;
using Game.MapSystems;
using Game.MapSystems.Enums;
using Game.Singleton;
using Game.Units;
using Game.Units.MODs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.AI
{
    [RequireComponent(typeof(Unit))]
    public class BotAI : MonoBehaviour, IRaundAction
    {
        public bool useSmartBotType = true;
        public BotType typeBot = BotType.Disable;
        private bool botActive = true;
        private Unit unit;
        private MapChanksController chanksController;
        private RaundTickerUnitMOD raundTicker;
        private Queue<LiverChank> historyWay = new Queue<LiverChank>();
        private int maxLenghtHistoryWay = 3;

        void IRaundAction.OnEndRaund() {}

        void IRaundAction.OnStartRaund()
        {
            if (botActive == false || typeBot == BotType.Disable 
                || unit.senor.Team != Singleton<GameManager>.MainSingleton.ActiveTeam)
                return;

            DoActionsAI();
        }

        private void DoActionsAI()
        {
            if (!DoVeritificate())
                return;

            if (typeBot == BotType.AlgoritmPassive)
                StartCoroutine(DoActionWithFixedDelay(() => TryAttack()));

            if (typeBot == BotType.AlgoritmActive)
                StartCoroutine(DoActionWithFixedDelay(() => Mover()));
        }

        private void Mover()
        {
            if (TryAttack())
                return;

            Unit un;
            if (chanksController.TryFindUnitOnMap("King", GameTeam.Blue, out un))
                TryMove(un);
        }

        private void Start()
        {
            unit = GetComponent<Unit>();
            chanksController = Singleton<MapSystem>.MainSingleton.ChanksController;
            raundTicker = unit.GetComponent<RaundTickerUnitMOD>();

            typeBot = Singleton<GameManager>.MainSingleton.TypeBotsAI;

            if (useSmartBotType)
                SmartTypeDetected();
        }

        private void SmartTypeDetected()
        {
            if (unit.senor.Team == GameTeam.Red)
                botActive = true;
            if (unit.senor.Team == GameTeam.Blue)
                botActive = false;
        }

        private bool TryAttack()
        {
            var movment = unit.GetComponent<UnitMovmentMOD>();
            if (movment == null)
                return false;

            if (TryActiveAction<ActionUnitAttackWithMoveMOD>(movment))
                return true;
            if (TryActiveAction<ActionUnitDistanceAttackMOD>(movment))
                return true;
            if (TryActiveAction<ActionUnitAttackWithoutMOD>(movment))
                return true;
            if (TryActiveAction<ActionUnitAttackWithDeadMOD>(movment))
                return true;

            return false;
        }

        private bool TryMove(Unit un)
        {
            var movment = unit.GetComponent<UnitMovmentMOD>();
            if (movment == null)
                return false;

            if (TryActiveActionMove<ActionUnitMoveMOD>(movment, un))
                return true;

            return false;
        }

        private bool TryActiveAction<T>(UnitMovmentMOD movment) where T : UnitMOD
        {
            var action = unit.GetComponent<T>();
            if (action == null)
                return false;

            var kingName = "King";
            if (action is IMovmentMOD mod)
            {
                var list = mod.GetListChanks();
                List<Chank> chanks = new List<Chank>();
                movment.AddInteractableChanksTo(ref chanks, mod);

                if (TryAttackKing(kingName, mod, chanks))
                    return true;

                foreach (var chank in chanks)
                {
                    if (!chank.ChankContainer.IsFree(unit.ChankQueue))
                    {
                        ActiveAction(mod, chank);

                        return true;
                    }
                }
            }

            return false;
        }

        private void ActiveAction(IMovmentMOD mod, Chank chank)
        {
            mod.OnMove(chank);
            var actionBaff = unit.GetComponent<BaffUnitMOD>();
            if (actionBaff != null)
                actionBaff.OnMove(chank);
        }

        private bool TryAttackKing(string kingName, IMovmentMOD mod, List<Chank> chanks)
        {
            foreach (var chank in chanks)
            {
                if (!chank.ChankContainer.IsFree(unit.ChankQueue) && chank.ChankContainer.ContainUnit(kingName))
                {
                    mod.OnMove(chank);
                    var actionBaff = unit.GetComponent<BaffUnitMOD>();
                    if (actionBaff != null)
                        actionBaff.OnMove(chank);
                    return true;
                }
            }
            return false;
        }

        private bool TryActiveActionMove<T>(UnitMovmentMOD movment, Unit un) where T : UnitMOD
        {
            var action = unit.GetComponent<T>();
            if (action == null)
                return false;

            CheckHistoryWay();

            List<Chank> dangerChanks = new List<Chank>();

            if (UnityEngine.Random.Range(0, 100) <= 70f)
                dangerChanks = FindDangerChanks();

            if (action is IMovmentMOD mod)
            {
                var list = mod.GetListChanks();
                List<Chank> chanks = new List<Chank>();

                movment.AddInteractableChanksTo(ref chanks, mod);

                var resAll = FindNearChanks(un.StandChank, chanks);

                foreach (var res in resAll)
                {
                    if (res != null && !HistoryContains(res) && !dangerChanks.Contains(res))
                    {
                        ActiveMove(mod, res);

                        return true;
                    }
                }
            }

            return false;
        }

        private void ActiveMove(IMovmentMOD mod, Chank res)
        {
            historyWay.Enqueue(new LiverChank(res, 3));

            mod.OnMove(res);
            var actionBaff = unit.GetComponent<BaffUnitMOD>();
            if (actionBaff != null)
                actionBaff.OnMove(res);
        }

        private List<Chank> FindDangerChanks()
        {
            var unteamUnits = chanksController.GetAllUnteamUnits(unit.senor.Team);
            List<Chank> dangerChanks = new List<Chank>();
            foreach (var unteamU in unteamUnits)
            {
                var attack = unteamU.GetComponent<ActionAttackMOD>();

                if (attack is IMovmentMOD atMod)
                {
                    var list = atMod.GetListChanks();
                    if (list != null)
                        dangerChanks.AddRange(list);
                }
            }

            return dangerChanks;
        }

        private List<Chank> FindNearChanks(Chank chank, List<Chank> chanks)
        {
            var copy = new Chank[chanks.Count];
            chanks.CopyTo(copy);
            List<Chank> result = copy.ToList();
            var sortedList = result
                .OrderBy(x => (chank.CoordinateCurrent - x.CoordinateCurrent).magnitude)
                .ToList();

            return sortedList;
        }

        private void CheckHistoryWay()
        {
            if (historyWay.Count > maxLenghtHistoryWay)
                historyWay.Dequeue();


            List<LiverChank> list = new List<LiverChank>();
            foreach (var live in historyWay)
            {
                live.timer -= 1;
                if (live.timer <= 0)
                    list.Add(live);
            }

            foreach (var l in list)
                historyWay.Dequeue();

        }

        private bool HistoryContains(Chank chank)
        {
            foreach (var live in historyWay)
                if (live.chank.Equals(chank))
                    return true;

            return false;
        }
        private bool DoVeritificate()
        {
            if (raundTicker != null)
                return raundTicker.CanDoMove();

            return false;
        }
        private IEnumerator DoActionWithFixedDelay(Action action)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            action();
        }

        class LiverChank
        {
            public Chank chank;
            public int timer;

            public LiverChank(Chank chank, int timer)
            {
                this.chank = chank;
                this.timer = timer;
            }
        }
    }
}
