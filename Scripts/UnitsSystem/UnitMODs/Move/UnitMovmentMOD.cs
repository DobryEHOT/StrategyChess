using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Units;

using Assets.Scripts.UnitsSystem.States;
using Assets.Scripts.SelectorSpace;
using Assets.Scripts.UnitsSystem.Interfaces;
using Game.Singleton;
using Game.MapSystems;
using Game.CardSystem.MOD;
using Game.CardSystem;
using Game.MapSystems.MOD;
using System.Linq;

namespace Game.Units.MODs
{
    public class UnitMovmentMOD : MonoBehaviour, IUnitAction
    {
        private SpawnerToyMOD spawnerToy;
        private Chank oldChank;
        private ChankSelectorMOD oldChankSelector;
        private PrepositionObjController prePos;
        private Vector3 pawnPosition;
        private SelectorItems selector;
        private List<IMovmentMOD> movmetMODs = new List<IMovmentMOD>();

        public List<Chank> ChanksMoving { get; protected set; } = new List<Chank>();
        public Unit RootUnit { get; protected set; }

        private void Start()
        {
            RootUnit = GetComponent<Unit>();
            spawnerToy = RootUnit.GetCardPrefab().GetComponent<SpawnerToyMOD>();
            prePos = RootUnit.GetComponent<PrepositionObjController>();
            selector = RootUnit.senor.GetComponent<SelectorItems>();
            movmetMODs = new List<IMovmentMOD>(GetComponents<IMovmentMOD>());
        }

        public void DoAfterAction(Chank chank)
        {
            if (chank != null)
                MovePawnToChank(chank);

            Singleton<MapSystem>.MainSingleton.ChanksController.DisableSelectorChanksColor();
        }

        public void DoBeforeAction()
        {
            oldChank = RootUnit.StandChank;
            ClearMap();
        }
        private void ClearMap()
        {
            var newChanksMoving = new List<Chank>();
            foreach (var moveMOD in movmetMODs)
            {
                ChanksMoving = moveMOD.GetListChanks();
                if (ChanksMoving != null && ChanksMoving.Count > 0)
                {
                    foreach (var chank in ChanksMoving)
                    {
                        if (chank == null)
                            continue;

                        if (moveMOD.isInteractable(chank))
                        {
                            newChanksMoving.Add(chank);
                            Singleton<MapSystem>.MainSingleton.ChanksController.ActiveChank(chank, moveMOD.GetColorChank());
                        }
                    }
                }
            }

            ChanksMoving = newChanksMoving;
        }

        public void AddInteractableChanksTo(ref List<Chank> newChanksMoving, IMovmentMOD moveMOD)
        {
            ChanksMoving = moveMOD.GetListChanks();
            if (ChanksMoving != null && ChanksMoving.Count > 0)
            {
                foreach (var chank in ChanksMoving)
                {
                    if (chank == null)
                        continue;

                    if (moveMOD.isInteractable(chank))
                        newChanksMoving.Add(chank);
                }
            }
        }

        private void MovePawnToChank(Chank chank)
        {
            var doMove = GetComponents<IMovmentOnMoveMOD>();

            int minPriority = 10;
            var dirInteractables = new Dictionary<IMovmentMOD, bool>();
            foreach (var d in doMove)
            {
                if (d is IMovmentMOD mover)
                {
                    dirInteractables.Add(mover, !mover.isInteractable(chank));
                    var priority = mover.GetPriority();
                    if (minPriority > priority)
                        minPriority = priority;
                }
            }


            foreach (var dover in doMove)
            {
                if (dover is IMovmentMOD mover)
                {
                    if ((dirInteractables[mover]) || !ChanksMoving.Contains(chank))
                        continue;

                    mover.OnMove(chank);
                }
                else
                {
                    dover.OnMove(chank);
                }
            }
        }

        private void PreUsePawn(Chank chank)
        {
            if (ChanksMoving.Contains(chank))
            {
                oldChank = chank;
                pawnPosition = Vector3.Lerp(RootUnit.transform.position, selector.GetPositionCursoure(), Time.deltaTime * 5);
                return;
            }

            var pos = RootUnit.GetDefaultPositionToChank(chank);
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

    public interface IMovmentOnMoveMOD
    {
        void OnMove(Chank chank);
        int GetPriority();
    }

    public interface IMovmentMOD : IMovmentOnMoveMOD
    {
        Material GetColorChank();
        List<Chank> GetListChanks();
        bool isInteractable(Chank chank);
    }
}
