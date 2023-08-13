using Assets.Scripts.StateMachin;
using Game.MapSystems.MOD;
using Game.Units;
using Game.MapSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Game.CardSystem;
using Game.CardSystem.MOD;
using Game.Singleton;
using Game.Units.MODs;
using Assets.Scripts.UnitsSystem.Interfaces;
using Assets.Scripts.SelectorSpace;

namespace Assets.Scripts.UnitsSystem.States
{
    public class UnitState : State
    {
        protected Unit rootUnit;

        public UnitState(Unit unit) => rootUnit = unit;

        public override void Enter(StateMachine machine) { }

        public override void Exit(StateMachine machine) { }

        public override void Update(StateMachine machine) { }
    }

    public class IdleUnitState : UnitState
    {
        public IdleUnitState(Unit unit) : base(unit) => spawnerToy = unit.GetCardPrefab().GetComponent<SpawnerToyMOD>();
        private SpawnerToyMOD spawnerToy;
        private Vector3 defaultPosition;

        public override void Enter(StateMachine machine) => defaultPosition = spawnerToy.GetDefaultPosition(rootUnit.StandChank);

        public override void Update(StateMachine machine)
        {
            var pos = rootUnit.transform.position;
            if (Vector3.SqrMagnitude(pos - defaultPosition) > 0)
                rootUnit.transform.position = Vector3.Lerp(pos, defaultPosition, 5 * Time.deltaTime);
        }

        public override void Exit(StateMachine machine) { }
    }

    public class SelectUnitState : UnitState
    {
        public SelectUnitState(Unit unit) : base(unit) { }
    }

    public class KeepUnitState : UnitState
    {
        public KeepUnitState(Unit unit) : base(unit) { }
        private IUnitAction[] actions;
        private Vector3 pawnPosition;
        private SelectorItems selector;
        private Dictionary<Chank, List<IUnitAction>> chanksActions = new Dictionary<Chank, List<IUnitAction>>();
        private bool isVerificated = true;

        public override void Enter(StateMachine machine)
        {
            DoVeritificate();
            if (!isVerificated)
            {
                machine.SwichTo<IdleUnitState>();
                return;
            }

            MapSystem.MainSingleton.ChanksController.DisableSelectorChanks();
            if (selector == null)
                selector = rootUnit.senor.GetComponent<SelectorItems>();

            actions = rootUnit.GetComponents<IUnitAction>();
            if (actions != null && actions.Length > 0)
            {
                foreach (var mod in actions)
                {
                    mod.DoBeforeAction();
                    TryAddMod(mod);
                }
            }

            selector.ActiveVizor = false;
        }

        public override void Exit(StateMachine machine)
        {
            if (!isVerificated)
                return;

            selector.ActiveVizor = true;

            RaycastHit hit;
            if (selector.TrySelect(out hit))
            {
                var chank = hit.transform.GetComponent<Chank>();
                actions = rootUnit.GetComponents<IUnitAction>();
                if (actions != null && actions.Length > 0)
                    foreach (var mod in actions)
                        mod.DoAfterAction(chank);
            }

            chanksActions.Clear();
            MapSystem.MainSingleton.ChanksController.DisableSelectorChanks();
        }

        public override void Update(StateMachine machine)
        {
            if (TryExitState(machine))
                return;

            EndTime(machine);
            Keep();
        }

        private bool TryExitState(StateMachine machine)
        {
            if (Input.GetMouseButtonUp(0) || !isVerificated)
            {
                machine.SwichTo<IdleUnitState>();
                MapSystem.MainSingleton.ChanksController.RecalculateChankTeams();
                return true;
            }

            return false;
        }

        private void Keep()
        {
            if (TrySelect())
                return;

            pawnPosition = Vector3.Lerp(rootUnit.transform.position, selector.GetPositionCursoure(), Time.deltaTime * 5f);
            rootUnit.transform.position = pawnPosition;
        }

        private bool TrySelect()
        {
            RaycastHit hit;
            if (selector.TrySelect(out hit))
            {
                var chank = hit.transform.GetComponent<Chank>();
                if (chanksActions != null && chanksActions.Count > 0)
                {
                    if (chanksActions.ContainsKey(chank))
                    {
                        foreach (var mod in chanksActions[chank])
                            mod.DoUpdateAction(chank);

                        return true;
                    }
                }
            }

            return false;
        }

        private void EndTime(StateMachine manipulator)
        {
            if (rootUnit.senor.Team == Singleton<Game.GameManager>.MainSingleton.ActiveTeam)
                return;

            manipulator.SwichTo<IdleUnitState>();
        }

        private void DoVeritificate()
        {
            var raund = rootUnit.GetComponent<RaundTickerUnitMOD>();
            if (raund != null)
                isVerificated = raund.CanDoMove();
        }

        private void TryAddMod(IUnitAction mod)
        {
            if (mod is IUnitActionMap map)
            {
                var list = map.GetListChanks();
                if (list != null || list.Count > 0)
                    CheckModContainer(mod, list);
            }
        }

        private void CheckModContainer(IUnitAction mod, List<Chank> list)
        {
            foreach (var element in list)
            {
                if (element != null)
                {
                    if (!chanksActions.ContainsKey(element))
                    {
                        var l = new List<IUnitAction>();
                        chanksActions.Add(element, l);
                        l.Add(mod);
                    }
                    else
                    {
                        var l = chanksActions[element];
                        if (!l.Contains(mod))
                            l.Add(mod);
                    }
                }
            }
        }
    }

    public class ActionUnitState : UnitState
    {
        public ActionUnitState(Unit unit) : base(unit) { }

        public override void Enter(StateMachine machine) { }

        public override void Exit(StateMachine machine) { }

        public override void Update(StateMachine machine) => machine.SwichTo<IdleUnitState>();
    }
}
