using System.Collections.Generic;
using UnityEngine;


namespace Game.CameraController
{
    public class GameResours : MonoBehaviour
    {
        public string Name { get; private set; }
        public int Count { get; private set; }
        private List<GameObject> resourceGameObject = new List<GameObject>();
        private GameObject prefabObject;
        private Transform parentSpawn;

        public GameResours(string name, GameObject prefab, Transform parent)
        {
            Name = name;
            parentSpawn = parent;
            prefabObject = prefab;
        }

        public GameResours(string name, GameObject prefab, Transform parent, int count) : this(name, prefab, parent) => AddResouce(count);

        public List<GameObject> GetListObjects() => resourceGameObject;

        public void AddResouce(int count, Vector3 position = new Vector3())
        {
            for (var i = 0; i < count; i++)
            {
                var obj = Instantiate(prefabObject, parentSpawn);
                obj.transform.position = position;
                resourceGameObject.Add(obj);
            }

            Count = resourceGameObject.Count;
        }

        public void RemoveResources(int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (resourceGameObject.Count > 0)
                {
                    var obj = resourceGameObject[0];
                    resourceGameObject.Remove(obj);
                    Destroy(obj);
                }
            }

            Count = resourceGameObject.Count;
        }

        public void RemoveAllResources()
        {
            var count = resourceGameObject.Count;
            RemoveResources(count);
        }
    }
}

