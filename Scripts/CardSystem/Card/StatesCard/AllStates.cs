using Game.MapSystems;
using Game.MapSystems.MOD;
using Game.Units;
using Game.Singleton;
using UnityEngine;
using Game.Sound;
using Assets.Scripts.SelectorSpace;

namespace Game.CardSystem.StatesCard
{
    class IdleCardState : CardState
    {
        public IdleCardState(Player player) : base(player) { }

        public override void DoAction(CardMover mover, StatesManipulator manipulator)
        {
            mover.transform.position = Vector3.Lerp(mover.transform.position, mover.GetPositionCardInHand(), Time.deltaTime * mover.SpeedInterpolation);
        }

        public override void EnterState(CardMover mover, StatesManipulator manipulator)
        {
            var b = mover.GetComponent<Card>().Billlbord;
            if (b != null)
                b.gameObject.SetActive(false);
        }

        public override void ExitState(CardMover mover, StatesManipulator manipulator) { }
    }

    class SelectCardState : CardState
    {
        private LayerMask mask;
        private string nameSound = "Knock";

        public SelectCardState(Player player) : base(player)
        {
            mask = LayerMask.GetMask("Card");
        }

        public override void DoAction(CardMover mover, StatesManipulator manipulator)
        {
            if (lochalPlayerTeam != player.Team)
                return;

            mover.transform.position = Vector3.Lerp(mover.transform.position, mover.GetPositionCardSelected(), Time.deltaTime * mover.SpeedInterpolation);
            Ray ray = player.MainCamera.ScreenPointToRay(Input.mousePosition);
            ray.direction *= 2f;
            TryKeepCard(mover, manipulator, ray);
        }

        private void TryKeepCard(CardMover mover, StatesManipulator manipulator, Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mask) && hit.collider.gameObject == mover.gameObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var card = mover.GetComponent<Card>();
                    var activeTeam = Singleton<GameManager>.MainSingleton.ActiveTeam;
                    if (activeTeam == player.Team && card.CanBuy())
                    {
                        mover.gameObject.GetComponent<BoxCollider>().enabled = false;
                        manipulator.SwichTo<KeepCardState>();
                    }
                }
            }
            else
            {
                manipulator.SwichTo<IdleCardState>();
            }
        }

        public override void EnterState(CardMover mover, StatesManipulator manipulator)
        {
            var b = mover.GetComponent<Card>().Billlbord;
            if (b != null)
                b.gameObject.SetActive(true);

            Singleton<SoundSys>.MainSingleton.TryPlaySound(nameSound, mover.transform.position);
        }

        public override void ExitState(CardMover mover, StatesManipulator manipulator)
        {
            var b = mover.GetComponent<Card>().Billlbord;
            if (b != null)
                b.gameObject.SetActive(false);
        }
    }

    class KeepCardState : CardState
    {
        private LayerMask maskCard;
        private LayerMask maskChank;
        private Vector3 cardPosition;
        private Card card;
        private Unit unit;
        private ChankSelectorMOD oldChankSelector;
        private SelectorItems selector;
        private Card lastSwitchedCard;
        private string takeSound = "BuyCard";
        private string useSound = "Close";

        public KeepCardState(Player player) : base(player)
        {
            maskCard = LayerMask.GetMask("Card");
            maskChank = LayerMask.GetMask("Chank");
        }

        public override void DoAction(CardMover mover, StatesManipulator manipulator)
        {
            EndTime(mover, manipulator);
            MouseUp(mover, manipulator);
            MouseGet(mover);
            mover.transform.position = cardPosition;
        }

        private void MouseGet(CardMover mover)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = GetCameraRay();
                RaycastHit hit;
                TrySwitchCard(mover, ray);
                if (Physics.Raycast(ray, out hit, 100, maskChank) && hit.collider.tag != "HandProtection")
                {
                    PreUseCard(mover, hit);
                }
                else
                {
                    if (oldChankSelector != null)
                        MapSystems.MapSystem.MainSingleton.ChanksController.ActiveFreeChank(oldChankSelector);

                    oldChankSelector = null;
                    cardPosition = Vector3.Lerp(mover.transform.position, mover.GetPositionCardInCursoure(), Time.deltaTime * mover.SpeedInterpolation);
                }
            }
        }

        private void TrySwitchCard(CardMover mover, Ray ray)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, maskCard))
            {
                if (lastSwitchedCard != null)
                    if (lastSwitchedCard.Equals(hit.collider.GetComponent<Card>()))
                        return;
                
                lastSwitchedCard = hit.collider.GetComponent<Card>();
                mover.Switch(lastSwitchedCard);
            }
        }

        private void PreUseCard(CardMover mover, RaycastHit hit)
        {
            if (!hit.transform.GetComponent<ChankContainerMOD>().IsFree(unit) || hit.transform.GetComponent<Chank>().ActiveTeam != mover.Player.Team)
            {
                cardPosition = Vector3.Lerp(mover.transform.position, mover.GetPositionCardInCursoure(), Time.deltaTime * mover.SpeedInterpolation);
                return;
            }
            if (!hit.transform.GetComponent<ChankSelectorMOD>().Equals(oldChankSelector))
            {
                if (oldChankSelector != null)
                    MapSystems.MapSystem.MainSingleton.ChanksController.ActiveFreeChank(oldChankSelector);

                oldChankSelector = hit.transform.GetComponent<ChankSelectorMOD>();
                MapSystems.MapSystem.MainSingleton.ChanksController.ActiveAimingChank(oldChankSelector);
            }

            cardPosition = Vector3.Lerp(mover.transform.position, hit.transform.position + Vector3.up + Vector3.forward, Time.deltaTime * mover.SpeedInterpolation);
            mover.GetComponent<PrepositionObjController>().PreUseCard(hit.transform.gameObject);
        }

        private void EndTime(CardMover mover, StatesManipulator manipulator)
        {
            if (mover.Player.Team == Singleton<GameManager>.MainSingleton.ActiveTeam)
                return;

            mover.gameObject.GetComponent<BoxCollider>().enabled = true;
            manipulator.SwichTo<IdleCardState>();
        }

        private void MouseUp(CardMover mover, StatesManipulator manipulator)
        {
            if (Input.GetMouseButtonUp(0))
            {
                selector.ActiveVizor = true;
                mover.gameObject.GetComponent<BoxCollider>().enabled = true;
                manipulator.SwichTo<IdleCardState>();
                Ray ray = GetCameraRay();
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, maskChank) && hit.collider.tag != "HandProtection")
                {
                    if (!hit.transform.GetComponent<ChankContainerMOD>().IsFree(unit) || hit.transform.GetComponent<Chank>().ActiveTeam != mover.Player.Team)
                        return;

                    UseCard(mover, hit);
                }
            }
        }

        private void UseCard(CardMover mover, RaycastHit hit)
        {
            mover.UseCard(hit.collider.GetComponent<Chank>());
            card.Buy();
            MapSystems.MapSystem.MainSingleton.ChanksController.RecalculateChankTeams();
            Singleton<SoundSys>.MainSingleton.TryPlaySound(useSound, mover.transform.position);
        }

        private Ray GetCameraRay()
        {
            Ray ray = player.MainCamera.ScreenPointToRay(Input.mousePosition);
            ray.direction *= 2f;
            Debug.DrawRay(ray.origin, ray.direction);
            return ray;
        }

        public override void EnterState(CardMover mover, StatesManipulator manipulator)
        {
            card = mover.GetComponent<Card>();
            unit = card.GetUnit();
            if (card != null)
            {
                MapSystems.MapSystem.MainSingleton.ChanksController.ActiveTeamSelectorChanks(card, player.Team);
                MapSystems.MapSystem.MainSingleton.ChanksController.ActiveZonesSelectorChanks(card, player.Team);
            }

            Singleton<SoundSys>.MainSingleton.TryPlaySound(takeSound, mover.transform.position);
            if (selector == null)
                selector = mover.Player.GetComponent<SelectorItems>();

            selector.ActiveVizor = false;
        }

        public override void ExitState(CardMover mover, StatesManipulator manipulator)
        {
            MapSystems.MapSystem.MainSingleton.ChanksController.DisableSelectorChanks();
            MapSystems.MapSystem.MainSingleton.ChanksController.RecalculateChankTeams();
            selector.ActiveVizor = true;
        }
    }
}
