using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class WindowVizor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Image;
        [SerializeField] private TextMeshPro windowName;
        [SerializeField] private TextMeshPro windowDescription;
        public void InjectItem(VizorItem item)
        {
            if (item == null)
                return;

            Image.sprite = item.image;
            windowName.text = item.itemName;
            windowDescription.text = item.itemDescription;
        }
    }
}
