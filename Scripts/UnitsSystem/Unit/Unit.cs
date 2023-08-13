using Game.CardSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Units.MODs;
using Game.Units.MOD;
using Game.MapSystems;
using Assets.Scripts.SelectorSpace;
using Assets.Scripts.UnitsSystem.States;
using Game.Singleton;
using Game.CardSystem.MOD;

namespace Game.Units
{
    public class Unit : MonoBehaviour, ISelectItem
    {
        public object ISelectItem { get; internal set; }
        public Player senor;
        public UnitMaskActions maskActions;
        public Chank StandChank;
        public bool IsAffectZoneMap = false;
        public bool IsDinamicUnit = false;
        [Space] [Range(0, 5)] public int ChankQueue;
        [SerializeField] private GameObject cardPref;
        public string nameUnit = "Default";
        private ActionMask action;
        private UnitStateMachine unitStateMachine;
        private bool selected = false;
        private SpawnerToyMOD spawnerToy;

        private void Awake()
        {
            InitVar();
            InitMODs();
        }

        private void Start()
        {
            unitStateMachine = new UnitStateMachine(GetComponent<Unit>());
            spawnerToy = GetCardPrefab().GetComponent<SpawnerToyMOD>();
        }

        private void Update() => unitStateMachine.DoUpdate();

        public virtual void Dead() => StandChank.ClearPawn(ChankQueue);

        public Vector3 GetDefaultPositionToChank(Chank chank) => spawnerToy.GetDefaultPosition(chank);

        public GameObject GetCardPrefab() => cardPref;

        public void OverrideActionMaskToName(string name)
        {
            nameUnit = name;
            action = maskActions.GetActionMask(nameUnit);
            InitMODs();
        }

        public void SwitchStandChank(Chank newChank)
        {
            if (newChank != null)
                newChank.SetPawnToChank(gameObject);

            if (StandChank != null)
                StandChank.ClearPawnToChank(gameObject);

            StandChank = newChank;
        }

        public bool TryDropPosition()
        {
            if (unitStateMachine.ActiveState is IdleUnitState)
            {
                unitStateMachine.SwichTo<ActionUnitState>();
                return true;
            }
            return false;
        }

        private void InitVar()
        {
            if (cardPref == null)
                throw new System.Exception("carf prefab is null");

            maskActions = UnitMaskActions.MainSingleton;
            action = maskActions.GetActionMask(nameUnit);
        }

        private void InitMODs()
        {
            var allComponents = GetComponents<UnitMOD>();
            if (allComponents == null || allComponents.Length == 0)
                return;

            foreach (var component in allComponents)
                component.SetMaskActions(action);
        }

        void ISelectItem.OnAimSelectItem(SelectorItems selector)
        {
            if (selected)
                return;

            unitStateMachine.SwichTo<SelectUnitState>();
        }

        void ISelectItem.OnSelectItem(SelectorItems selector)
        {
            if (!IsDinamicUnit || (senor.Team != Singleton<GameManager>.MainSingleton.ActiveTeam
                || (selector.Player != null && selector.Player.Team != senor.Team)))
                return;

            selected = true;
            unitStateMachine.SwichTo<KeepUnitState>();
        }

        void ISelectItem.OnUnAimSelectItem(SelectorItems selector) { }

        void ISelectItem.OnUnselectItem(SelectorItems selector) { }
    }
}
