using Game.Global;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlaceDecorator : MonoBehaviour
{
    [SerializeField] private List<GameObject> Decor = new List<GameObject>();
    void Start()
    {
        var global = Singleton<GlobalManager>.MainSingleton;
        if (global == null || Decor.Count == 0)
            return;

        if (Decor.Count > global.Level - 1)
        {
            var dec = Decor[global.Level - 1];
            dec.SetActive(true);
        }
    }
}
