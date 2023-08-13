using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraController
{
    public class PlayerResources : MonoBehaviour
    {
        private Dictionary<ResourceType, GameObject> resourcesPrefabs;
        private Dictionary<ResourceType, GameResours> resourcesObjects;

        [Header("Prefabs")]
        [SerializeField] private GameObject goldPrefab;
        [SerializeField] private GameObject ironPrefab;
        [SerializeField] private GameObject moralityPrefab;

        [Header("Settings")]
        [SerializeField] private Transform startPositionResources;
        [SerializeField] private float bettween = 10f;
        [SerializeField] private Transform flagBackgraund;
        [SerializeField] private float yOffSetflag = 1f;
        [SerializeField] private float speedMove = 5f;
        [SerializeField] [Range(1, 20)] private int maxCount = 18;
        [SerializeField] [Range(1, 20)] private int oneCount = 6;

        private void Awake() //Переписать ресурсы через ScriptableObjects
        {
            resourcesPrefabs = new Dictionary<ResourceType, GameObject>()
            {
                { ResourceType.Gold, goldPrefab},
                { ResourceType.Iron, ironPrefab},
                { ResourceType.Morality, moralityPrefab}
            };

            resourcesObjects = new Dictionary<ResourceType, GameResours>()
            {
                { ResourceType.Gold, new GameResours("Gold", goldPrefab, startPositionResources,0)},
                { ResourceType.Iron, new GameResours("Iron", ironPrefab, startPositionResources,0)},
                { ResourceType.Morality, new GameResours("Morality", moralityPrefab, startPositionResources,0)}
            };
        }

        private void Update() //Не оптимизированное решение
        {
            int downOffset = 0;
            RecalculateResourcePositions(out downOffset);
            RecalculateFlagPosition(downOffset);
        }

        private void RecalculateFlagPosition(int downOffSet)
        {
            if (flagBackgraund != null)
            {
                var offsetFlag = startPositionResources.position + (bettween * downOffSet * -startPositionResources.up) + yOffSetflag * startPositionResources.up;
                var offSetFlagMultiplayer = 0.5f;
                offsetFlag += offSetFlagMultiplayer * startPositionResources.forward;
                var speedFlag = 10f;
                flagBackgraund.position = Vector3.Lerp(flagBackgraund.position, offsetFlag, Time.deltaTime * speedFlag);
            }
        }

        private void RecalculateResourcePositions(out int downOffSet)
        {
            var y = 0;
            foreach (var element in resourcesObjects)
            {
                var list = element.Value.GetListObjects();
                if (list != null && list.Count > 0)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var posEnd = startPositionResources.position + (-startPositionResources.up * bettween * (i + y));
                        var posStart = list[i].transform.position;

                        list[i].transform.position = Vector3.Lerp(posStart, posEnd, Time.deltaTime * speedMove);
                    }
                }
                y += list.Count;
            }
            downOffSet = y;
        }

        public List<ResourceType> GetListResources()
        {
            var list = new List<ResourceType>();
            foreach (var res in resourcesObjects)
                if (res.Value.Count > 0)
                    list.Add(res.Key);

            return list;
        }

        public bool HaveResource(ResourceType resource, int count)
        {
            var res = resourcesObjects[resource];
            if (res.Count >= count)
                return true;

            return false;
        }

        public void AddResource(ResourceType resource, int count, Vector3 position)
        {
            if (maxCount <= GetCountAllResources())
                return;

            var res = resourcesObjects[resource];
            res.AddResouce(CanAddCountResource(resource, count), position);
        }

        public void AddResource(ResourceType resource, int count)
        {
            if (maxCount <= GetCountAllResources())
                return;

            var res = resourcesObjects[resource];
            res.AddResouce(CanAddCountResource(resource, count));
        }

        public void RemoveResource(ResourceType resource, int count)
        {
            var res = resourcesObjects[resource];
            res.RemoveResources(count);
        }

        public void DropAllResource()
        {
            foreach (var rest in resourcesObjects)
                rest.Value.RemoveAllResources();
        }

        private int GetCountAllResources()
        {
            var countRes = 0;
            foreach (var ress in resourcesObjects)
                countRes += ress.Value.Count;

            return countRes;
        }

        private int CanAddCountResource(ResourceType resource, int wantAddCount)
        {
            var have = resourcesObjects[resource].Count;
            var max = have + wantAddCount;
            if (max > oneCount)
                return Mathf.Clamp(max, 0, oneCount) - have;
            else
                return wantAddCount;
        }
    }

    public enum ResourceType
    {
        Gold,
        Iron,
        Morality
    }
}