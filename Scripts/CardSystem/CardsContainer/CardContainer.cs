using Game.CameraController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CardSystem
{
    public class CardContainer : MonoBehaviour
    {
        [SerializeField] private List<Card> Container;
        public int Count { get => Container.Count; }

        public Card GetRandomCard()
        {
            var i = Random.Range(0, Container.Count - 1);
            if (Container.Count > 0)
                return Container[i];

            return null;
        }

        public Card GetRandomCardHaveResourcesPrice(List<ResourceType> res)
        {
            var list = new List<Card>();
            foreach (var card in Container)
                if (CardÑontainsPrice(card, res))
                    list.Add(card);

            var i = Random.Range(0, list.Count - 1);
            if (list.Count > 0)
                return list[i];

            return null;
        }

        public Card GetRandomCardHaveResourcesPrice(List<ResourceType> res, List<string> nameCards)
        {
            var list = new List<Card>();
            foreach (var card in Container)
                if (CardÑontainsPrice(card, res) && (nameCards != null && nameCards.Contains(card.Name)))
                    list.Add(card);

            var i = Random.Range(0, list.Count - 1);
            if (list.Count > 0)
                return list[i];

            return null;
        }

        public List<Card> GetContainer() => Container;

        public void SetCard(Card card) => Container.Add(card);

        public void DelitCard(Card card) => Container.Remove(card);

        public void DelitFirstCard()
        {
            if (Container.Count <= 0)
                return;

            var element = Container[0];
            Container.Remove(element);
            Destroy(element.gameObject);
        }

        public void SwichCards(Card one, Card two)
        {
            if (one == null || two == null)
                return;
            if (!Container.Contains(one) && !Container.Contains(two))
                throw new System.Exception("This card container dosen't have such cards");

            Card middle = one;

            var indexOne = Container.IndexOf(one);
            var indexTwo = Container.IndexOf(two);

            if (indexOne == -1 || indexTwo == -1)
                return;

            Container[indexOne] = two;
            Container[indexTwo] = middle;
        }

        public Card GetCard(string NameCard)
        {
            foreach (var card in Container)
                if (card.Name.Equals(NameCard))
                    return card;

            throw new System.Exception("A card with that name does not exist");
        }

        private bool CardÑontainsPrice(Card card, List<ResourceType> res)
        {
            foreach (var price in card.PriceList)
            {
                bool have = false;
                foreach (var need in res)
                    if (price.type == need)
                        have = true;

                if (!have)
                    return false;
            }

            return true;
        }

        public IEnumerator GetEnumerator() => Container.GetEnumerator();

        public Card this[int i]
        {
            get => Container[i];
            set => Container[i] = value;
        }
    }
}