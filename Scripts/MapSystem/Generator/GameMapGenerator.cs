using Game.MapSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.MapSystems.Enums;
using UnityEngine;

namespace Game.MapSystems.Generator
{
    public static class GameMapGenerator
    {
        static float between = 1;

        public static PlayerMap CreatePlayerMap(GameObject chank, Transform startPosition, GameTeam team, int width, int height)
        {
            var player = new PlayerMap(team, width, height, between);
            player.Generate(chank, startPosition);

            return player;
        }
    }

    public class PlayerMap
    {
        public Dictionary<int, Chank> Map { get; private set; } = new Dictionary<int, Chank>();
        private GameTeam team;
        private int width;
        private int height;
        private float between;

        public PlayerMap(GameTeam gameTeam, int width, int height, float between)
        {
            team = gameTeam;
            this.width = width;
            this.height = height;
            this.between = between;
        }

        public void Generate(GameObject chank, Transform startPosition)
        {
            int number = 0;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var obj = GameObject.Instantiate(chank, startPosition.position 
                        + (startPosition.forward * y * between) 
                        + (startPosition.right * x * between), startPosition.rotation);
                    obj.transform.SetParent(startPosition);
                    var ch = obj.GetComponent<Chank>();
                    ch.SetSpawnInfo(number, team, width, height, new Vector2(x, y));

                    Map.Add(number, obj.GetComponent<Chank>());
                    number++;
                }
            }
        }
    }

    public class GameMap
    {
        private Dictionary<int, Chank> allMaps = new Dictionary<int, Chank>();
        private List<GameTeam> allTeams = new List<GameTeam>();

        public int Width { get; private set; }
        public int Height { get; private set; }

        public GameMap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void AddMap(PlayerMap map)
        {
            for (var i = 0; i < map.Map.Count; i++)
            {
                var element = map.Map[i];

                if (element.Team == GameTeam.Blue)
                {
                    allMaps.Add(i, element);
                    element.CoordinateCurrent = element.Coordinate;
                }
                else if (element.Team == GameTeam.Red)
                {
                    allMaps.Add(i + Width * Height, element);
                    element.CoordinateCurrent = element.CoordinateSecond;
                }
                else
                {
                    continue;
                }

                if (!allTeams.Contains(element.Team))
                {
                    allTeams.Add(element.Team);
                }
            }
        }

        public Chank GetRandomTeamChank(GameTeam team)
        {
            List<Chank> chanks = GetTeamChanks(team);

            var i = UnityEngine.Random.Range(0, chanks.Count - 1);
            if (i > 0)
                return chanks[i];

            return null;
        }

        public List<Chank> GetTeamChanks(GameTeam team)
        {
            List<Chank> chanks = new List<Chank>();
            foreach (var element in allMaps)
                if (element.Value.Team == team)
                    chanks.Add(element.Value);

            return chanks;
        }

        public Chank GetFirstChankTeam(GameTeam team)
        {
            var zero = Vector3.zero;
            foreach (var element in allMaps)
                if (element.Value.Coordinate.Equals(zero) && element.Value.Team == team)
                    return element.Value;

            return null;
        }

        public Chank GetChank(int number)
        {
            if (number >= allMaps.Count)
                throw new Exception("This number bigger, than have a map");

            return allMaps[number];
        }

        public int GetCountChanks() => allMaps.Count;

        public int GetCountTeams() => allTeams.Count;

        public Chank GetChank(int x, int y)
        {
            foreach (var chank in allMaps)
                if (chank.Value.CoordinateCurrent == new Vector2(x, y))
                    return chank.Value;
            

            return null;
        }

        public Chank this[int x, int y]
        {
            get => GetChank(x, y);
        }

        public Chank this[int x]
        {
            get => GetChank(x);
        }

        public IEnumerator<KeyValuePair<int, Chank>> GetEnumerator()
        {
            var enumer = allMaps.GetEnumerator();

            while (enumer.MoveNext())
                yield return enumer.Current;
        }
    }
}
