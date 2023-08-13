using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.CardSystem;
using Game.MapSystems;
using Game.Singleton;
using Game.Global;

namespace Game.CardSystem.HandManipulation
{
    [RequireComponent(typeof(CardContainer))]
    public class HandCards : MonoBehaviour
    {
        [SerializeField] private Transform startPosition;
        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private Transform spawnContainer;

        [SerializeField] private float distanceBetweenCards = 1f;

        private const int MaxHandCards = 10;
        private Player player;
        private bool hidedCards = false;

        public List<string> DeckCards = new List<string>();
        public CardContainer Container { get; private set; }

        private void Awake()
        {
            Container = GetComponent<CardContainer>();
        }

        private void Start()
        {
            player = GetComponent<Player>();
            GetLevelNameCards();
        }

        public bool HandIsFull() => MaxHandCards <= Container.Count;

        public void RemoveCardToDeckCards(string nameCard)
        {
            DeckCards.Remove(nameCard);
        }

        public void SwichCard(Card one, Card two)
        {
            Container.SwichCards(one, two);
        }

        public Card TakeCard(string NameCard)
        {
            if (MaxHandCards <= Container.Count)
                return null;

            var card = SpawnCard(NameCard);
            Container.SetCard(card);
            card.Take();

            RecalculateCardsPosition();
            return card;
        }
        public void UseCard(Card card, Chank chank)
        {
            if (card == null || chank == null)
                return;

            Container.DelitCard(card);
            card.Use(chank, player);
            Destroy(card.gameObject);
            RecalculateCardsPosition();
        }

        public void DelitFirstCard() => Container.DelitFirstCard();

        public void DropAllCards()
        {
            var con = Container.GetContainer();
            if (con != null)
            {
                foreach (var c in con)
                    if (c != null)
                        Destroy(c.gameObject);

                con.Clear();
            }
        }

        public void HideCards()
        {
            if (hidedCards)
                return;

            hidedCards = true;
            spawnContainer.transform.position -= player.MainCamera.transform.up * 3f;

            RecalculateCardsPosition();
        }

        public void DeHideCards()
        {
            if (!hidedCards)
                return;

            hidedCards = false;
            spawnContainer.transform.position += player.MainCamera.transform.up * 3f;

            RecalculateCardsPosition();
        }


        private void GetLevelNameCards()
        {
            var levelStratage = Singleton<GlobalManager>.MainSingleton.GetLevelStratage();
            string[] lis = new string[levelStratage.CardOnLevel.Count];
            levelStratage.CardOnLevel.CopyTo(lis);
            DeckCards.AddRange(lis);
        }

        private void RecalculateCardsPosition()
        {
            if (Container.Count != 0)
            {
                for (var i = 0; i < Container.Count; i++)
                {
                    var id = i + ((MaxHandCards - Container.Count) / 2);
                    var mover = Container[i].GetComponent<CardMover>();
                    mover.SetHandPosition(spawnContainer, id, distanceBetweenCards, this);
                }
            }
        }

        private Card SpawnCard(string NameCard)
        {
            var card = CardSystem.MainSingleton.GetCard(NameCard);
            var obj = GameObject.Instantiate(card, startPosition.position, spawnContainer.rotation);
            obj.transform.SetParent(spawnContainer);

            var mover = obj.GetComponent<CardMover>();
            mover.SetHandPosition(spawnContainer, Container.Count, distanceBetweenCards, this);
            var resultCard = obj.GetComponent<Card>();
            CheckOnAninimCard(resultCard);

            return resultCard;
        }

        private void CheckOnAninimCard(Card card)
        {
            if (card.GetComponent<CardMover>().Player.Team != Singleton<LochalClientInformation>.MainSingleton.Info.Team)
            {
                var obj = Instantiate(Singleton<CardSystem>.MainSingleton.AnonimPrefabCard);
                var chilCount = card.gameObject.transform.childCount;
                for (var i = 0; i < chilCount; i++)
                    card.gameObject.transform.GetChild(i).gameObject.SetActive(false);

                obj.transform.SetParent(card.gameObject.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}