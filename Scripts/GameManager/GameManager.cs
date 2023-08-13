using Assets.Scripts.GameManager.States;
using Game.MapSystems.Enums;
using Game.Singleton;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Global;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        public Game.MapSystems.MapSystem MapSystemGame { get; private set; }
        public List<Player> AllPlayers { get; private set; } = new List<Player>();
        public Dictionary<GameTeam, List<Player>> TeamPlayers = new Dictionary<GameTeam, List<Player>>();
        public int RaundSecond { get; private set; } = 60;
        public GameTeam ActiveTeam { get; private set; }

        public bool PauseIsActive = false;

        public BotType TypeBotsAI = BotType.AlgoritmPassive; 

        [SerializeField] public List<TeamInfo> teamInfos = new List<TeamInfo>();

        public Objective GameObjective { get; private set; }
        public GameStatesController GameStatesController { get; private set; }

        void Awake()
        {
            Inicialize(this);
        }

        private void Start()
        {
            MapSystemGame = Game.MapSystems.MapSystem.MainSingleton;
            GameStatesController = GetComponent<GameStatesController>();

            var levelStratage = Singleton<GlobalManager>.MainSingleton.GetLevelStratage();
            GameObjective = levelStratage.GameObjective;
            GameObjective.Reset();
            GameObjective.SetStatesController(GameStatesController);
            GameStatesController.SetMachine(new GameStateMachine(this));
        }

        public void SetActiveTeam(GameTeam activeTeam)
        {
            ActiveTeam = activeTeam;
        }

        public void AddPlayer(Player player)
        {
            if (player == null)
                return;

            AllPlayers.Add(player);
            var team = player.Team;
            if (TeamPlayers.ContainsKey(team))
            {
                if (TeamPlayers[team].Contains(player))
                    return;
            }
            else
            {
                TeamPlayers.Add(team, new List<Player>());
            }

            TeamPlayers[team].Add(player);
        }

        public List<Player> GetPlayersList()
        {
            return AllPlayers;
        }

        public bool TryGetTeamInfo(GameTeam team, out TeamInfo teamInfo)
        {
            foreach (var info in teamInfos)
            {
                if (info.Team == team)
                {
                    teamInfo = info;
                    return true;
                }
            }

            teamInfo = default;
            return false;
        }
    }

    [Serializable]
    public struct TeamInfo
    {
        [SerializeField] public GameTeam Team;
        [SerializeField] public string teamName;
        [SerializeField] public Color teamColor;
    }
}
