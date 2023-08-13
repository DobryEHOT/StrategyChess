using Assets.Scripts.SelectorSpace;
using Game;
using Game.Singleton;
using Game.UI;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class SenderCheckToObjective : MonoBehaviour
{
    private void OnDestroy()
    {
        var unit = GetComponent<Unit>();
        if (unit == null)
            return;

        var sen = unit.senor;
        if (sen == null)
            return;

        Singleton<MainScreen>.MainSingleton.SetActiveIcons(false);
        Singleton<GameManager>.MainSingleton.GameObjective.Check(unit);
    }
}
