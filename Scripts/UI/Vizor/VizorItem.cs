using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "VizorItem", menuName = "ScriptableObjects/VizorItem", order = 1)]
    public class VizorItem : ScriptableObject
    {
        public Sprite image;
        public string itemName;
        public string itemDescription;
    }
}
