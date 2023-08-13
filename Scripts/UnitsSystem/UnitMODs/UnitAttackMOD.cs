using Assets.Scripts.SelectorSpace;
using Assets.Scripts.UnitsSystem.Interfaces;

using Game.CardSystem;
using Game.CardSystem.MOD;
using Game.MapSystems;
using Game.MapSystems.MOD;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    public class UnitAttackMOD : UnitMOD, IUnitAction, IUnitActionMap
    {
        public string DefaultModsName = "MoveAttack";
        private SpawnerToyMOD spawnerToy;
        private Vector3 pawnPosition;
        private SelectorItems selector;

        private void Start()
        {
            spawnerToy = RootUnit.GetCardPrefab().GetComponent<SpawnerToyMOD>();
            selector = RootUnit.senor.GetComponent<SelectorItems>();
        }

        public void DoAfterAction(Chank chank)
        {
            MovePawnToChank(chank);
            Singleton<MapSystem>.MainSingleton.ChanksController.DisableSelectorChanks();
        }

        public void DoBeforeAction()
        {
            ChanksMoving = GetListChanks();
            ClearMap();
            Singleton<MapSystem>.MainSingleton.ChanksController.ActivePawnMaps(this, ChankSelector.Attack);
        }

        private void ClearMap()
        {
            var newChanksMoving = new List<Chank>();
            if (ChanksMoving != null && ChanksMoving.Count > 0)
            {
                foreach (var chank in ChanksMoving)
                {
                    if (chank == null)
                        continue;

                    var container = chank.ChankContainer;

                    Unit unit;
                    if (container.TryGetUnit(RootUnit.ChankQueue, out unit))
                        if (RootUnit.senor.Team != unit.senor.Team)
                            newChanksMoving.Add(chank);
                }
            }

            ChanksMoving = newChanksMoving;
        }

        protected override void InitUnitMOD() => ModsName = DefaultModsName;

        private void MovePawnToChank(Chank chank)
        {
            if (!ChanksMoving.Contains(chank))
                return;

            chank.ClearPawn(RootUnit.ChankQueue);
            var pos = spawnerToy.GetDefaultPosition(chank);
            RootUnit.transform.position = pos;
            RootUnit.SwitchStandChank(chank);

            var raund = GetComponent<RaundTickerUnitMOD>();
            if (raund != null)
                raund.DoMoveMOD();
        }

        private void PreUsePawn(Chank chank)
        {
            if (ChanksMoving.Contains(chank))
            {
                pawnPosition = Vector3.Lerp(RootUnit.transform.position, selector.GetPositionCursoure(), Time.deltaTime * 5);
                return;
            }

            var pos = spawnerToy.GetDefaultPosition(chank);
            pawnPosition = Vector3.Lerp(RootUnit.transform.position, pos, Time.deltaTime * 5);
        }

        public void DoUpdateAction(Chank chank)
        {
            if (chank != null)
            {
                PreUsePawn(chank);
                RootUnit.transform.position = pawnPosition;
            }
        }
    }
}
