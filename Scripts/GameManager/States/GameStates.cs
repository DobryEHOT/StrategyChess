using Assets.Scripts.StateMachin;
using Game.Singleton;
using Game.Ticker;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.MapSystems.Enums;
using UnityEngine;
using Game.MapSystems.MOD;
using Assets.Scripts.UnitsSystem.Interfaces;
using Game.Global;
using Game.CardSystem;
using Game.MapSystems;
using System.Collections;
using Game.AI;
using Assets.Scripts.SelectorSpace;
using Game.Units;

namespace Assets.Scripts.GameManager.States
{
    public abstract class GameState : State
    {
        protected Game.GameManager manager;
        public GameState(Game.GameManager gameManager)
        {
            manager = gameManager;
        }

        public override void Enter(StateMachine machine) { }

        public override void Exit(StateMachine machine) { }

        public override void Update(StateMachine machine) { }
    }

    public class CreateGameState : GameState
    {
        private bool mapRedy = false;

        public CreateGameState(Game.GameManager gameManager) : base(gameManager) { }

        public override void Enter(StateMachine machine)
        {
            manager.StartCoroutine(WaitTime(FullingMap, 1f));
        }

        private void FullingMap()
        {
            var list = manager.GetPlayersList();
            foreach (var player in list)
            {
                if (player.Team == GameTeam.Red)
                {
                    var levelStratage = Singleton<GlobalManager>.MainSingleton.GetLevelStratage();
                    var sec = levelStratage.GenerateLevelItems(Singleton<MapSystem>.MainSingleton, player);
                    manager.StartCoroutine(WaitTime(DoRedy, sec));
                }
            }
        }

        public override void Update(StateMachine machine)
        {
            if (mapRedy && manager.MapSystemGame.MapRedy)
                machine.SwichTo<StartGameState>();
        }

        private void DoRedy()
        {
            mapRedy = true;
        }

        protected IEnumerator WaitTime(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }

    public class StartGameState : GameState
    {
        public StartGameState(Game.GameManager gameManager) : base(gameManager) { }

        public override void Enter(StateMachine machine)
        {
            var list = manager.GetPlayersList();
            foreach (var player in list)
            {
                if (player.Team == GameTeam.Blue)
                    SetStartCards(player);
            }
        }

        private static void SetStartCards(Game.Player player)
        {
            var hand = player.PlayerHand;

            hand.TakeCard("King");
        }

        public override void Update(StateMachine machine)
        {
            machine.SwichTo<ProcessGameState>();
        }
    }

    public class ProcessGameState : GameState
    {
        private Dictionary<GameTeam, List<Player>>.KeyCollection keysAllTeams;
        private IEnumerator<GameTeam> keysEnumerator;
        private GameTeam activeTeam;
        private TickableItem timeItem;
        private int time;
        private int counter;
        public ProcessGameState(Game.GameManager gameManager) : base(gameManager) { }

        public override void Enter(StateMachine machine)
        {
            manager.TypeBotsAI = BotType.AlgoritmActive;

            if (manager.TeamPlayers.Count > 0)
            {
                keysAllTeams = manager.TeamPlayers.Keys;
                keysEnumerator = keysAllTeams.GetEnumerator();
                PassTheTurnQueue();
            }
        }

        public override void Update(StateMachine machine)
        {
            if (counter >= 1 / Time.fixedDeltaTime)
            {
                TimeUpdateUI();
                if (time <= 0)
                    PassTheTurnQueue();

                counter = 0;
            }

            counter++;
        }

        public void ForcePassMove()
        {
            PassTheTurnQueue();
        }

        private void TimeUpdateUI()
        {
            List<Player> list = manager.AllPlayers;

            foreach (var element in list)
            {
                var rController = element.GetComponent<RaundController>();
                if (rController != null)
                    rController.SetTime(time);
            }

            if (!manager.PauseIsActive)
                time--;
        }

        private void PassTheTurnQueue()
        {
            RaundTickerUpdate(RaundNeed.End);
            SwitchNextActiveTeam();
            ShowActualUI();
            UpdateCardSystem();
            DropResource();
            RaundTickerUpdate(RaundNeed.Start);
            UpdateAI();

            time = manager.RaundSecond;
            counter = 0;
        }

        private void SwitchNextActiveTeam()
        {
            if (!keysEnumerator.MoveNext())
            {
                keysEnumerator.Reset();
                keysEnumerator.MoveNext();
            }

            activeTeam = keysEnumerator.Current;
            manager.SetActiveTeam(activeTeam);
        }

        private void UpdateAI()
        {
            List<Player> list = manager.AllPlayers;
            foreach (var p in list)
            {
                var comp = p.GetComponent<OutputActions>();
                if (comp != null)
                    comp.OnStartRaund();
            }
        }

        private void DropResource()
        {
            List<Player> list = manager.AllPlayers;
            foreach (var pl in list)
            {
                if (pl.Team == activeTeam)
                    pl.Resources.DropAllResource();
            }
        }

        private static void UpdateCardSystem()
        {
            Singleton<CardSystem>.MainSingleton.GetCardOnStartRaund();
        }

        private void RaundTickerUpdate(RaundNeed need)
        {
            List<Player> list = manager.AllPlayers;
            var map = Game.MapSystems.MapSystem.MainSingleton.ChanksController.GetMap();
            foreach (var chank in map)
            {
                var container = chank.Value.GetComponent<ChankContainerMOD>();
                foreach (var slot in container.GetUnitSlots())
                {
                    ActiveRaundAction(need, slot);
                }
            }
        }

        private static void ActiveRaundAction(RaundNeed need, KeyValuePair<int, Unit> slot)
        {
            var unit = slot.Value;
            if (unit != null)
            {
                var units = unit.GetComponents<IRaundAction>();
                if (units != null && units.Length > 0)
                {
                    foreach (var u in units)
                    {
                        if (need == RaundNeed.Start)
                            u.OnStartRaund();
                        if (need == RaundNeed.End)
                            u.OnEndRaund();
                    }
                }
            }
        }

        private void ShowActualUI()
        {
            List<Player> list = manager.AllPlayers;

            if (list.Count > 0)
                foreach (var element in list)
                    ShowPlayerUI(element);
        }

        private void ShowPlayerUI(Player element)
        {
            var rController = element.GetComponent<RaundController>();
            if (rController != null)
            {
                if (element.Team == manager.ActiveTeam)
                    rController.ShowYourMove();
                else
                    rController.ShowMoveEnemy();

                rController.ShowRotateTime();
                rController.SetTime(time);
                rController.StartShowTime();
            }
        }

        enum RaundNeed
        {
            Start,
            End
        }
    }

    public class OwnerMoveGameState : GameState
    {
        public OwnerMoveGameState(Game.GameManager gameManager) : base(gameManager) { }

    }

    public class OwnerWaitGameState : GameState
    {
        public OwnerWaitGameState(Game.GameManager gameManager) : base(gameManager) { }
    }

    public class EndGameState : GameState
    {
        public EndGameState(Game.GameManager gameManager) : base(gameManager) { }

        public override void Enter(StateMachine machine)
        {
            ShowActualUI();
            ClearHandPlayers();
        }

        private void ClearHandPlayers()
        {
            List<Player> list = manager.AllPlayers;
            foreach (var player in list)
                player.PlayerHand.DropAllCards();
        }

        private void ShowActualUI()
        {
            List<Player> list = manager.AllPlayers;

            if (list.Count > 0)
            {
                foreach (var element in list)
                {
                    if (element == null)
                        continue;
                    var rController = element.GetComponent<RaundController>();
                    if (rController != null)
                    {
                        if (element.Team == manager.GameObjective.WinTeam)
                            rController.ShowWin();
                        else
                            rController.ShowLoose();

                        var selector = element.GetComponent<SelectorItems>();
                        if (selector != null)
                            selector.ActiveVizor = false;
                    }
                }
            }
        }
    }
}
