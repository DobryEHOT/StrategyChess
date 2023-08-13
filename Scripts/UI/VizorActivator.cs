using Assets.Scripts.SelectorSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VizorActivator : MonoBehaviour
    {
        [SerializeField] private SelectorItems selector;
        [SerializeField] private GameObject ActiveObj;
        [SerializeField] private GameObject DisableObj;

        public void SwitchActive()
        {
            if (selector == null)
                return;

            var activeObj = selector.ActiveVizorAlways;
            selector.ActiveVizorAlways = activeObj ? false : true;

            ActiveObj.SetActive(!activeObj);
            DisableObj.SetActive(activeObj);
        }
    }
}
