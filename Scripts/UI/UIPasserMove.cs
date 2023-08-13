using Assets.Scripts.GameManager.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Singleton;
using Game;

public class UIPasserMove : MonoBehaviour
{
    [SerializeField] private Player player;

    public void DoPass()
    {
        if (Singleton<GameStatesController>.MainSingleton.Active is ProcessGameState active)
        {
            if (Singleton<GameManager>.MainSingleton.ActiveTeam == player.Team)
                active.ForcePassMove();
        }
    }
}
