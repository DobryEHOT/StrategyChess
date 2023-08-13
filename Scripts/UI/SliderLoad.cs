using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Slider))]
    public class SliderLoad : MonoBehaviour
    {
        [SerializeField] private string nameLoad;

        void Awake()
        {
            var slider = GetComponent<Slider>();
            string data;
            if (Singleton<SaveSystem>.MainSingleton.TryGetDataToName(nameLoad, out data))
            {
                float value;
                if (float.TryParse(data, out value))
                    slider.value = value;
            }
        }
    }
}
