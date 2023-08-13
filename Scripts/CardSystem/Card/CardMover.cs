using Assets.Scripts.SelectorSpace;
using Game.CardSystem.HandManipulation;
using Game.CardSystem.StatesCard;
using Game.MapSystems;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CardSystem
{
    [RequireComponent(typeof(Card))]
    public class CardMover : MonoBehaviour, ISelectItem
    {
        public float SpeedInterpolation { get; private set; } = 10;
        public Player Player { get; private set; }

        private Camera mainCamera;
        private HandCards hand;

        private Transform handPosition;
        private float offsetCardHand;

        private StatesManipulator manipulator;
        private ClientInfo clientInfo;
        public int NumberHand { get; private set; }

        private void Start()
        {
            clientInfo = Singleton<LochalClientInformation>.MainSingleton.Info;
        }

        void Update()
        {
            manipulator.DoUpdate();
        }

        public void SetHandPosition(Transform hPos, int number, float offsetHand, HandCards handCards)
        {
            handPosition = hPos;
            NumberHand = number;
            offsetCardHand = offsetHand;
            hand = handCards;

            if (Player == null)
            {
                Player = hand.GetComponent<Player>();
                manipulator = new StatesManipulator(this, Player);
                mainCamera = Player.MainCamera;
            }
        }

        public void Switch(Card card)
        {
            if (card == null)
                return;

            var newCardMover = card.GetComponent<CardMover>();
            var newNum = newCardMover.NumberHand;
            newCardMover.SetHandPosition(handPosition, NumberHand, offsetCardHand, hand);
            NumberHand = newNum;
            hand.SwichCard(card, GetComponent<Card>());
        }

        public void UseCard(Chank chank) => hand.UseCard(GetComponent<Card>(), chank);

        public Vector3 GetPositionCardInHand()
        {
            return handPosition.position 
                + (-handPosition.right * offsetCardHand * NumberHand) 
                + (handPosition.forward * 0.03f * NumberHand) 
                + (handPosition.up * -0.07f * NumberHand);
        }

        public Vector3 GetPositionCardSelected()
        {
            var upOffSet = 1.6f;
            var positiopnUpOffSet = mainCamera.transform.up * upOffSet;
            var positionRightOffSet = new Vector3(offsetCardHand * NumberHand, 0, 0);

            return handPosition.position + positionRightOffSet + positiopnUpOffSet;
        }

        public Vector3 GetPositionCardInCursoure()
        {
            var a = mainCamera.ScreenPointToRay(Input.mousePosition);
            return a.origin + (a.direction * 6f);
        }

        private void OnDestroy()
        {
            manipulator.CloseManipulator();
        }

        void ISelectItem.OnAimSelectItem(SelectorItems selector)
        {
            if (clientInfo.Team == Player.Team)
                manipulator.DoSelected();
        }
        void ISelectItem.OnSelectItem(SelectorItems selector) { }

        void ISelectItem.OnUnAimSelectItem(SelectorItems selector) { }

        void ISelectItem.OnUnselectItem(SelectorItems selector) { }
    }
}