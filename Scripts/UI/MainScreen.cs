using Game.Singleton;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class MainScreen : Singleton<MainScreen>
    {
        [SerializeField] private GameObject chankVizor;

        private List<GameObject> icons = new List<GameObject>();

        void Awake()
        {
            Inicialize();
        }

        public GameObject GetChankVizor() => chankVizor;

        public void AddIconScreen(GameObject icon)
        {
            icons.Add(icon);
        }

        public void RemoveIconScreen(GameObject icon)
        {
            icons.Remove(icon);
        }

        public void SetActiveIcons(bool active)
        {
            foreach (var icon in icons)
                if (icon != null)
                    icon.SetActive(active);
        }
    }
}
