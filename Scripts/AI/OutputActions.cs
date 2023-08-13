using Assets.Scripts.GameManager.States;
using Game.CardSystem;
using Game.CardSystem.HandManipulation;
using Game.Global;
using Game.MapSystems;
using Game.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class OutputActions : MonoBehaviour
    {
        [SerializeField] private HandCards hand;
        private Player player;
        private LevelStratage levelStratage;
        private List<string[]> dec = new List<string[]>();
        private bool ticker = true;

        private List<string> dontStayWithThisUnits = new List<string>()
        {
            "MountainHight",
            "MountainLow",
            "Mountain",
            "Castle_House",
            "Castle_HouseCore",
            "Castle_Tower",
            "Castle_Wall",
            "River",
        };

        void Awake()
        {
            player = GetComponent<Player>();
        }

        private void Start()
        {
            levelStratage = Singleton<GlobalManager>.MainSingleton.GetLevelStratage();
            string[][] lis = new string[levelStratage.CardOnLevel.Count][];
            levelStratage.BotDecCard.CopyTo(lis);
            dec.AddRange(lis);
        }

        public void OnStartRaund()
        {
            if (player.Team != Singleton<GameManager>.MainSingleton.ActiveTeam)
                return;

            StartCoroutine(WaitTime(() => DoUseOtherCards(), 1f));
            StartCoroutine(WaitTime(() => DoPass(), 5f));
        }
        public void DoPass()
        {
            if (Singleton<GameStatesController>.MainSingleton.Active is ProcessGameState active)
            {
                if (Singleton<GameManager>.MainSingleton.ActiveTeam == player.Team)
                    active.ForcePassMove();
            }
        }

        private void DoUseOtherCards()
        {
            ticker = ticker ? false : true;
            if (ticker)
                return;

            if (dec.Count == 0)
                return;

            var el = dec[0];
            dec.Remove(el);

            if (el == null)
                return;

            var teamChanks = Singleton<MapSystem>.MainSingleton.ChanksController.GetTeamChanks(player.Team);
            var cardSys = Singleton<CardSystem.CardSystem>.MainSingleton;
            var dic2 = GetTrueSortedCards(el, teamChanks, cardSys);
            var cards = GetCardForUse(el, dic2);

            UseCards(cards, player);
        }

        private Dictionary<string, List<Chank>> GetTrueSortedCards(string[] el, List<Chank> teamChanks, CardSystem.CardSystem cardSys)
        {
            var dic2 = new Dictionary<string, List<Chank>>();
            foreach (var cardName in el)
            {
                if (!dic2.ContainsKey(cardName))
                    dic2.Add(cardName, new List<Chank>());

                List<Chank> ch1 = null;
                if (dic2.TryGetValue(cardName, out ch1))
                {
                    var card = cardSys.GetCard(cardName);
                    foreach (var chank in teamChanks)
                        if (!ContainsForWitiut(chank) && chank.ChankContainer.IsFree(card.GetUnit().ChankQueue))
                            ch1.Add(chank);
                }
            }

            return dic2;
        }

        private static List<CardInfo> GetCardForUse(string[] el, Dictionary<string, List<Chank>> dic2)
        {
            List<CardInfo> cards = new List<CardInfo>();
            foreach (var cardName in el)
            {
                List<Chank> ch1 = null;
                if (dic2.TryGetValue(cardName, out ch1))
                {
                    var ran = UnityEngine.Random.Range(0, ch1.Count - 1);
                    if (ch1.Count > 0 && ran >= 0 && ch1.Count - 1 > ran)
                    {
                        var red = ch1[ran];
                        cards.Add(new CardInfo(cardName, red));

                        foreach (var dd in dic2)
                            if (dd.Value.Contains(red))
                                dd.Value.Remove(red);
                    }
                }
            }

            return cards;
        }

        private bool ContainsForWitiut(Chank chank)
        {
            var d = chank.ChankContainer;
            var co = d.GetUnitSlots();
            foreach (var u in co)
                if (dontStayWithThisUnits.Contains(u.Value.nameUnit))
                    return true;

            return false;
        }

        public void RestartCards()
        {
            hand.DropAllCards();
            var card = hand.TakeCard("King");
            StartCoroutine(UseCards(card));
        }

        public void UseRandomCard()
        {
            var card = hand.Container.GetRandomCard();
            Chank chank = null;
            while (chank == null || !chank.ChankContainer.IsFree(0))
                chank = Singleton<MapSystem>.MainSingleton.ChanksController.GetRandomTeamChank(player.Team);

            hand.UseCard(card, chank);
        }

        IEnumerator UseCards(Card king)
        {
            var wait = new WaitForFixedUpdate();
            yield return wait;
            yield return wait;
            yield return wait;
            var ch = Singleton<MapSystem>.MainSingleton.ChanksController.GetFirstChankTeam(player.Team);
            if (ch.ChankContainer.IsFree(0))
            {
                hand.UseCard(king, ch);
            }
            else
            {
                yield return wait;
                yield return wait;
                yield return wait;
                Singleton<Manager>.MainSingleton.GameRestartMatch();
                yield break;
            }

            yield return wait;
            yield return wait;
            yield return wait;

            for (var i = 0; i < 9; i++)
            {
                UseRandomCard();
            }
        }

        protected void UseCard(Chank chank, Player player, string nameCard = "Default")
        {
            if (chank == null || player == null)
                return;

            var card = player.PlayerHand.TakeCard(nameCard);
            player.StartCoroutine(WaitTime(() => player.PlayerHand.UseCard(card, chank), 0.1f));
        }

        protected void UseCards(List<CardInfo> cards, Player player) => player.StartCoroutine(UseCardsMass(cards, player));

        private IEnumerator UseCardsMass(List<CardInfo> cards, Player player)
        {
            var waiter = new WaitForFixedUpdate();

            foreach (var card in cards)
            {
                for (var i = 0; i < 10; i++)
                    yield return waiter;

                if (card.chank != null && player != null)
                {
                    player.PlayerHand.DelitFirstCard();
                    yield return waiter;

                    var cardObj = Singleton<CardSystem.CardSystem>.MainSingleton.GetCard(card.NameCard);
                    if (cardObj == null)
                        continue;

                    if (card.chank.ChankContainer.IsFree(cardObj.GetUnit()))
                        UseCard(card.chank, player, card.NameCard);
                }
            }
        }

        protected IEnumerator WaitTime(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }
}
