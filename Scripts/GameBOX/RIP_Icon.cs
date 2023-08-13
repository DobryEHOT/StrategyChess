using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RIP_Icon : MonoBehaviour
{
    [SerializeField] private GameObject RIP;
    [SerializeField] private GameObject Def;
    [SerializeField] private int lvl = 6;
    [SerializeField] private int tom = 1;

    void Start()
    {
        var global = Singleton<SaveSystem>.MainSingleton;
        if (global == null || RIP == null || Def == null)
            return;

        if (global.TryGetAccessLevel(tom, lvl))
        {
            Def.SetActive(false);
            RIP.SetActive(true);
        }
    }
}
