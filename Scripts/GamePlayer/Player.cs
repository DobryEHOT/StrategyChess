using Game.CameraController;
using Game.CardSystem.HandManipulation;
using Game.MapSystems.Enums;
using Game.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(HandCards))]
    public class Player : MonoBehaviour
    {
        public HandCards PlayerHand { get; private set; }
        public GameTeam Team;
        public Camera MainCamera;
        public Camera ResourcesCamera;
        public PlayerResources Resources;
        private GameManager manager;
        [SerializeField] private List<Behaviour> activateMyComponentsPlayer = new List<Behaviour>();

        private void Awake()
        {
            PlayerHand = GetComponent<HandCards>();
            if (MainCamera == null)
                throw new System.Exception("You need set Camera for Player");
        }

        private void EnableLochalPlayerComonents()
        {
            if (Singleton<LochalClientInformation>.MainSingleton.Info.Team != Team)
                return;

            Singleton<LochalClientInformation>.MainSingleton.Info.player = this;
            foreach (var com in activateMyComponentsPlayer)
                com.enabled = true;
        }

        private void Start()
        {
            EnableLochalPlayerComonents();
            manager = Singleton<GameManager>.MainSingleton;
            manager.AddPlayer(this);
        }
    }
}
