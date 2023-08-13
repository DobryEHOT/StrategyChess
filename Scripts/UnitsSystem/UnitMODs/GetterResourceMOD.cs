using Assets.Scripts.UnitsSystem.Interfaces;
using Game;
using Game.CameraController;
using Game.MapSystems;
using Game.Singleton;
using Game.Units;
using Game.Units.MODs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetterResourceMOD : MonoBehaviour, IRaundAction
{
    [SerializeField] private List<ResourceType> resAdd = new List<ResourceType>();
    [SerializeField] private List<GetRes> resAddZone = new List<GetRes>();
    [SerializeField] private TypeGetter getter = TypeGetter.OnSpawn;
    [SerializeField] private bool considerTeamChank = true;

    private Player senor;
    private Unit unit;
    private GameManager gameManager;
    private LochalClientInformation lochalClient;
    private ActionUnitZoneMOD zone;

    public void OnEndRaund() { }

    public void OnStartRaund()
    {
        if (gameManager.ActiveTeam != lochalClient.Info.Team)
            return;

        if (getter == TypeGetter.OnRaundTickLochalTeamPlayer)
            TryGetResource();

        GetResourceWithZone(TypeGetter.OnRaundTickLochalTeamPlayer);
    }

    public List<GetRes> GetResourcesZone() => resAddZone;

    private void TryGetResource(GetRes res)
    {
        var chank = unit.StandChank;
        if (chank == null)
            return;

        if (considerTeamChank)
        {
            var chankActiveTeam = unit.StandChank.ActiveTeam;
            if (chankActiveTeam == Game.MapSystems.Enums.GameTeam.Neutral)
            {
                return;
            }
            else
            {
                foreach (var p in Singleton<GameManager>.MainSingleton.AllPlayers)
                {
                    if (p.Team == chankActiveTeam)
                    {
                        var resorses = res.Resource;
                        if (resorses != null)
                            foreach (var r in resorses)
                                GetResFromPlayer(chank, p, r);
                    }
                }
                return;
            }
        }

        GetResFromPlayer(chank, senor);
    }

    private void TryGetResource()
    {
        var chank = unit.StandChank;
        if (chank == null)
            return;

        if (considerTeamChank)
        {
            var chankActiveTeam = unit.StandChank.ActiveTeam;
            if (chankActiveTeam == Game.MapSystems.Enums.GameTeam.Neutral)
            {
                return;
            }
            else
            {
                foreach (var p in Singleton<GameManager>.MainSingleton.AllPlayers)
                    if (p.Team == chankActiveTeam)
                        GetResFromPlayer(chank, p);

                return;
            }
        }

        GetResFromPlayer(chank, senor);
    }

    private void GetResFromPlayer(Chank chank, Player senor)
    {
        if (senor.Resources == null)
            return;

        Vector3 posOnScreen = Vector3.zero;
        var chankPos = chank.transform.position;
        var camera = senor.MainCamera;
        if (camera != null)
        {
            posOnScreen = camera.WorldToScreenPoint(chankPos);

            var resCamera = senor.ResourcesCamera;
            if (resCamera != null)
                posOnScreen = resCamera.ScreenToWorldPoint(posOnScreen);
        }

        if (resAdd.Count > 0)
            foreach (var resItem in resAdd)
                senor.Resources.AddResource(resItem, 1, posOnScreen);
    }

    private void GetResFromPlayer(Chank chank, Player senor, ResourceType resource)
    {
        if (senor.Resources == null)
            return;

        Vector3 posOnScreen = Vector3.zero;
        var chankPos = chank.transform.position;
        var camera = senor.MainCamera;
        if (camera != null)
        {
            posOnScreen = camera.WorldToScreenPoint(chankPos);

            var resCamera = senor.ResourcesCamera;
            if (resCamera != null)
                posOnScreen = resCamera.ScreenToWorldPoint(posOnScreen);

        }

        senor.Resources.AddResource(resource, 1, posOnScreen);
    }

    private void GetResourceWithZone(TypeGetter When)
    {
        if (zone == null)
            return;

        foreach (var getter in resAddZone)
        {
            if (When == getter.When)
            {
                var units = zone.GetUnitsInZone();
                var needGet = NeedGetResource(getter, units);
                if (needGet)
                    TryGetResource(getter);
            }
        }
    }

    private static bool NeedGetResource(GetRes getter, List<Unit> units)
    {
        var needGet = false;

        if (units.Count <= 0 && !getter.UseZonePositive)
            return true;

        foreach (var unit in units)
        {
            if (getter.UseZoneNegative && getter.ZoneDontHave.Contains(unit.nameUnit))
                return false;

            if (getter.UseZonePositive && getter.ZoneHave.Count > 0)
            {
                if (getter.ZoneHave.Contains(unit.nameUnit))
                    needGet = true;
            }
            else
            {
                needGet = true;
            }
        }

        return needGet;
    }

    private void Start()
    {
        InitVar();
        if (unit == null)
            return;

        senor = unit.senor;
        if (senor == null)
            return;

        if (getter == TypeGetter.OnSpawn)
            TryGetResource();

        GetResourceWithZone(TypeGetter.OnSpawn);
    }

    private void InitVar()
    {
        zone = GetComponent<ActionUnitZoneMOD>();
        gameManager = Singleton<GameManager>.MainSingleton;
        lochalClient = Singleton<LochalClientInformation>.MainSingleton;
        unit = GetComponent<Unit>();
    }

    private void OnDestroy()
    {
        if (getter == TypeGetter.OnDestroy)
            TryGetResource();

        GetResourceWithZone(TypeGetter.OnDestroy);
    }

    public enum TypeGetter
    {
        OnSpawn,
        OnRaundTick,
        OnDestroy,
        OnRaundTickLochalTeamPlayer
    }

    [Serializable]
    public struct GetRes
    {
        public TypeGetter When;
        public List<ResourceType> Resource;

        public bool UseZonePositive;
        public List<string> ZoneHave;

        public bool UseZoneNegative;
        public List<string> ZoneDontHave;
    }
}
