using Game.CameraController;
using Game.CardSystem.Deligates;
using Game.CardSystem.MOD;
using Game.MapSystems;
using Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CardSystem
{
    public class Card : MonoBehaviour
    {
        [Header("ID")]
        public string Name;
        public int id;

        [Header("Price")]

        [SerializeField]
        public List<ResorceUnit> PriceList = new List<ResorceUnit>();

        private OnUse onUse;
        private OnTake onTake;

        public ResorcesCardBuillbord Billlbord;

        public void Use(Chank chank, Player player)
        {
            if (onUse != null)
                onUse(chank, player);
        }
        public void Take()
        {
            if (onTake != null)
                onTake();
        }

        public void Buy()
        {
            var mover = GetComponent<CardMover>();
            var player = mover.Player;
            var res = player.Resources;
            if (mover == null || player == null || res == null || !CanBuy())
                return;

            foreach (var element in PriceList)
                res.RemoveResource(element.type, element.price);
        }

        public bool CanBuy()
        {
            var mover = GetComponent<CardMover>();
            var player = mover.Player;
            var res = player.Resources;
            if (mover == null || player == null || res == null)
                return false;

            foreach (var element in PriceList)
                if (!res.HaveResource(element.type, element.price))
                    return false;

            return true;
        }

        public Unit GetUnit()
        {
            var spawner = GetComponent<SpawnerToyMOD>();
            if (spawner == null)
                return null;

            return spawner.GetUnit();
        }

        public void AddActionOnUse(OnUse actoin) => onUse += actoin;
        public void AddActionOnTake(OnTake actoin) => onTake += actoin;
    }

    [Serializable]
    public struct ResorceUnit
    {
        [SerializeField]
        public ResourceType type;

        [SerializeField]
        public int price;
    }
}


