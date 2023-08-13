using UnityEngine;

namespace Game.MapSystems.Generator
{
    public class PlayerMapGenerat : MonoBehaviour
    {
        [SerializeField] private GameObject chankPrefab;
        [SerializeField] private Transform StartPosition;
        [SerializeField] private Player player;
        private MapSystem map;
        private PlayerMap playerMap;

        void Start()
        {
            if (player == null)
                throw new System.Exception("You need set Player from Generator");

            if (chankPrefab == null)
                throw new System.Exception("You need set prefab to 'chankPrefan'!");

            map = MapSystem.MainSingleton;
            playerMap = GameMapGenerator.CreatePlayerMap(chankPrefab, StartPosition, player.Team, map.MapWidth, map.MapHeight);
            map.AddMap(playerMap);
        }
    }
}