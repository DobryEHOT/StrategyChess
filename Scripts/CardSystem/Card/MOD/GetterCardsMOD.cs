using Game.CardSystem.HandManipulation;
using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CardSystem.MOD
{
    [RequireComponent(typeof(CardMover))]
    public class GetterCardsMOD : CardMOD
    {
        [SerializeField]
        private List<string> GetCardsOnUse = new List<string>();
        private HandCards hand;
        protected override void OnStart()
        {
            var mover = GetComponent<CardMover>();
            if (mover == null)
                return;
            
            var player = mover.Player;
            if (player == null)
                return;

            hand = player.PlayerHand;
        }

        protected override void OnTake() { }

        protected override void OnUse(Chank chank, Player player)
        {
            if (hand == null)
                return;

            foreach (var cardName in GetCardsOnUse)
                hand.TakeCard(cardName);
        }
    }
}