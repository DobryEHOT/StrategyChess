using Assets.Scripts.UnitsSystem.Interfaces;
using Game.MapSystems;
using Game.Units.MODs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaundTickerUnitMOD : MonoBehaviour, IRaundAction
{
    public int MoveMODCount { get; set; } = 0;
    public int FineMove { get; private set; } = 0;

    private IconOnScreen icon;

    public bool CanDoMove()
    {
        if (MoveMODCount <= 0)
            return false;

        return true;
    }

    public void DoMoveMOD()
    {
        if (MoveMODCount <= 0)
            return;

        MoveMODCount--;

        RecalculateAlphaForIcon();
    }

    private void RecalculateAlphaForIcon()
    {
        if (icon == null)
            return;

        if (MoveMODCount > 0)
            icon.SetAlpha(1f);
        else
            icon.SetAlpha(0.4f);
    }

    public void OnEndRaund() { }

    public void OnMove(Chank chank)
    {
        DoMoveMOD();
    }

    public void OnStartRaund()
    {
        if (MoveMODCount < 1)
            MoveMODCount++;

        RecalculateAlphaForIcon();
    }

    void Start()
    {
        icon = GetComponent<IconOnScreen>();
    }
}
