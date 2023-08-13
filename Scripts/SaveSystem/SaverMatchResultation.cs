using Game;
using Game.Global;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaverMatchResultation : MonoBehaviour
{
    public void SaveLevelSuccessfully() => Singleton<GlobalManager>.MainSingleton.SaveActiveLvlAccess(true);
}
