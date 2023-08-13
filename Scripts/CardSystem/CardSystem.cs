using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Singleton;
using Game.CameraController;
using Game.Global;
using Game.MapSystems;

namespace Game.CardSystem
{
    [RequireComponent(typeof(CardContainer))]
    public class CardSystem : Singleton<CardSystem>
    {
        public GameObject AnonimPrefabCard;

        private CardContainer container;
        private GameManager manager;

        private void Awake()
        {
            Inicialize(this);
            container = GetComponent<CardContainer>();
        }

        private void Start()
        {
            manager = Singleton<GameManager>.MainSingleton;
        }

        public Card GetCard(string Name) => container.GetCard(Name);

        public void GetCardOnStartRaund()
        {
            var players = manager.AllPlayers;
            foreach (var player in players)
            {
                if (player.Team == manager.ActiveTeam)
                {
                    var card = GetRandomCardHaveResourcesPrice(player.Resources.GetListResources(), player.PlayerHand.DeckCards);

                    if (card != null)
                    {
                        if (!player.PlayerHand.HandIsFull())
                        {
                            player.PlayerHand.TakeCard(card.Name);
                            player.PlayerHand.RemoveCardToDeckCards(card.Name);
                        }
                    }
                }
            }
        }

        private Card GetRandomCard() => container.GetRandomCard();
        private Card GetRandomCardHaveResourcesPrice(List<ResourceType> res, List<string> nameCards) => container.GetRandomCardHaveResourcesPrice(res, nameCards);
    }
}
