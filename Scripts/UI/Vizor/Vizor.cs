using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class Vizor : MonoBehaviour
    {
        [SerializeField] private WindowVizor[] poolInfoWindows;
        [SerializeField] private float distanceBetweenWindows = 1f;
        public void SetVizorItems(List<VizorItem> items)
        {
            if (items == null)
                return;

            ResetPoolPositions();

            int i = 0;
            foreach (var item in items)
            {
                InjectItem(item, i);
                RecalculateWindowPosition(i);
                i++;
            }
        }

        private void RecalculateWindowPosition(int i)
        {
            if (poolInfoWindows.Length <= i)
                return;

            var oldPos = poolInfoWindows[i].transform.localPosition;
            poolInfoWindows[i].transform.localPosition = oldPos + new Vector3(0, (-distanceBetweenWindows * i) + distanceBetweenWindows, 0);
        }

        private void ResetPoolPositions()
        {
            if (poolInfoWindows != null)
            {
                foreach (var info in poolInfoWindows)
                {
                    info.gameObject.SetActive(false);
                    info.transform.localPosition = Vector3.zero;
                }
            }
        }

        private void InjectItem(VizorItem item, int index)
        {
            if (poolInfoWindows.Length <= index)
                return;

            var window = poolInfoWindows[index];
            window.InjectItem(item);
            window.gameObject.SetActive(true);
        }
    }
}
