using Game.MapSystems;
using Game.MapSystems.Enums;
using Game.MapSystems.Generator;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapSystems
{
    public class MapSystem : Singleton<MapSystem>
    {
        [SerializeField] private ScrObjMapSettings settings;
        public int MapWidth { get => settings.mapWidth; }
        public int MapHeight { get => settings.mapHeight; }
        public bool MapRedy { get; protected set; }
        public MapChanksController ChanksController { get; protected set; }

        private GameMap map;

        protected void Awake()
        {
            Inicialize(this);
            map = new GameMap(MapWidth, MapHeight);
        }

        public void AddMap(PlayerMap playerMap)
        {
            map.AddMap(playerMap);

            if (map.GetCountTeams() >= settings.countTeamWait)
                LastStageMapConstruct();
        }

        private void LastStageMapConstruct()
        {
            MapRedy = true;
            ChanksController = new MapChanksController(map);
        }
    }
}
