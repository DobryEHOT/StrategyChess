using Game.CameraController;
using Game.CardSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.CardSystem
{
    public class ResorcesCardBuillbord : MonoBehaviour
    {
        [SerializeField] private Card card;
        private List<ResorceUnitBullbord> list = new List<ResorceUnitBullbord>();
        private float between = -0.5f;

        [SerializeField] public List<ResorcePrefabs> prefabsList = new List<ResorcePrefabs>();
        private Dictionary<ResourceType, GameObject> resourcesPrefabs;

        [Header("Prefabs")]
        [SerializeField] private GameObject goldPrefab;
        [SerializeField] private GameObject ironPrefab;
        [SerializeField] private GameObject moralityPrefab;

        private void Awake() => card.Billlbord = this;

        private void Start()
        {
            InitResources();
            List<ResorceUnit> prices = InitPriceBoards();
            SetValuePrices(prices);
        }

        private void SetValuePrices(List<ResorceUnit> prices)
        {
            foreach (var element in list)
                foreach (var price in prices)
                    if (price.type == element.ResourceType)
                        element.SetValue(price.price);
        }

        private List<ResorceUnit> InitPriceBoards()
        {
            var prices = card.PriceList;
            for (var i = 0; i < prices.Count; i++)
            {
                var pref = resourcesPrefabs[prices[i].type];
                var obj = Instantiate(pref, transform);

                obj.transform.position = transform.position + (-transform.up * between * i);
                list.Add(obj.GetComponent<ResorceUnitBullbord>());
            }

            return prices;
        }

        private void InitResources()
        {
            resourcesPrefabs = new Dictionary<ResourceType, GameObject>()
            {
                { ResourceType.Gold, goldPrefab},
                { ResourceType.Iron, ironPrefab},
                { ResourceType.Morality, moralityPrefab}
            };
        }
    }

    [Serializable]
    public struct ResorcePrefabs
    {
        [SerializeField]
        public ResourceType type;

        [SerializeField]
        public GameObject prefab;
    }
}