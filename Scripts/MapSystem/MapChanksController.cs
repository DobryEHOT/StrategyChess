using Game.CardSystem;
using Game.MapSystems.Enums;
using Game.MapSystems.Generator;
using Game.MapSystems.MOD;
using Game.Units;
using Game.Units.MOD;
using Game.Units.MODs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.MapSystems
{
    public class MapChanksController
    {
        public MapChanksController(GameMap gameMap) => map = gameMap;
        private GameMap map;

        public List<Unit> GetAllTeamUnits(GameTeam team)
        {
            List<Unit> units = new List<Unit>();
            foreach (var chank in map)
                units.AddRange(chank.Value.ChankContainer.GetTeamUnits(team));

            return units;
        }

        public List<Unit> GetAllUnteamUnits(GameTeam team)
        {
            List<Unit> units = new List<Unit>();
            foreach (var chank in map)
                units.AddRange(chank.Value.ChankContainer.GetUnteamUnits(team));

            return units;
        }

        public bool TryFindUnitOnMap(string unitName, GameTeam team, out Unit unit)
        {
            foreach (var chank in map)
            {
                foreach (var un in chank.Value.ChankContainer.GetUnitSlots())
                {
                    if (un.Value.nameUnit.Equals(unitName) && un.Value.senor.Team == team)
                    {
                        unit = un.Value;
                        return true;
                    }
                }
            }

            unit = null;
            return false;
        }

        public GameMap GetMap() => map;
        public Chank GetRandomTeamChank(GameTeam team) => map.GetRandomTeamChank(team);
        public Chank GetFirstChankTeam(GameTeam team) => map.GetFirstChankTeam(team);

        public void DeAttachUnitsOnMap()
        {
            int i = 0;
            foreach (var chank in map)
            {
                i++;
                chank.Value.ChankContainer.ClearAllUnitSlots();
            }
        }

        public void DisableSelectorChanks()
        {
            var count = map.GetCountChanks();

            for (var i = 0; i < count; i++)
            {
                var chank = map[i];
                var container = chank.GetComponent<ChankContainerMOD>();
                if (container != null)
                    ActiveSelector(chank, ChankSelector.Disable);
            }
        }

        public void DisableSelectorChanksColor()
        {
            var count = map.GetCountChanks();

            for (var i = 0; i < count; i++)
            {
                var chank = map[i];
                var container = chank.GetComponent<ChankContainerMOD>();
                if (container != null)
                    ActiveSelector(chank, null, ChankSelector.Disable);
            }
        }

        public void ActiveChank(Chank chank, Material color)
        {
            if (chank != null)
                ActiveSelector(chank, color);
        }


        public void ActivePawnMaps(UnitMOD move, ChankSelector chankSelector = ChankSelector.Free)
        {
            var chanks = move.ChanksMoving;

            if (chanks != null && chanks.Count != 0)
                foreach (var chank in chanks)
                    ActiveSelector(chank, chankSelector);
        }

        public List<Chank> GetZoneChanks(UnitMOD mod)
        {
            List<Chank> list = new List<Chank>();
            var uModInfo = mod.GetUnitModInfo();
            if (uModInfo == null)
                return list;

            var uModMap = uModInfo.mapMask;
            if (uModMap == null)
                return list;

            var startCoord = mod.RootUnit.StandChank.CoordinateCurrent;

            list = GetZone(mod, uModInfo, uModMap, startCoord);
            return list;
        }

        public List<Chank> GetDistanceAttackChanks(UnitMOD move)
        {
            List<Chank> list = new List<Chank>();
            var uModInfo = move.GetUnitModInfo();
            if (uModInfo == null)
                return list;

            var uModMap = uModInfo.mapMask;
            if (uModMap == null)
                return list;

            var startCoord = move.RootUnit.StandChank.CoordinateCurrent;
            var resultTypeMap = new TypeMoveUnit[uModInfo.Width, uModInfo.Height];

            ApplyMapWitoutHeight(move, list, uModInfo, uModMap, startCoord, ref resultTypeMap);

            return list;
        }

        public List<Chank> GetMoveChanks(UnitMOD move)
        {
            List<Chank> list = new List<Chank>();
            var uModInfo = move.GetUnitModInfo();
            if (uModInfo == null)
                return list;

            var uModMap = uModInfo.mapMask;
            if (uModMap == null)
                return list;

            var startCoord = move.RootUnit.StandChank.CoordinateCurrent;
            var resultTypeMap = new TypeMoveUnit[uModInfo.Width, uModInfo.Height];
            ApplyMap(move, list, uModInfo, uModMap, startCoord, ref resultTypeMap);

            var way = GetPawnOpenWay(resultTypeMap, move);
            list = new List<Chank>();
            ApplyMap(move, list, uModInfo, way, startCoord, ref resultTypeMap);

            return list;
        }

        private void ApplyMap(UnitMOD move, List<Chank> list, UnitModInfo uModInfo,
            TypeMoveUnit[,] uModMap, Vector2 startCoord, ref TypeMoveUnit[,] resultTypeMap)
        {
            for (var x = 0; x < uModInfo.Width; x++)
            {
                for (var y = 0; y < uModInfo.Height; y++)
                {
                    if (uModMap[x, y] == TypeMoveUnit.Move)
                    {
                        var vX = x + (int)startCoord.x - uModInfo.Width / 2;
                        var vY = y + (int)startCoord.y - uModInfo.Height / 2;

                        if ((vX > map.Width || vY > map.Height * 2 || vX < 0 || vY < 0))
                            continue;

                        var chank = map[vX, vY];
                        if (chank == null)
                            continue;

                        if (move.UseAdditialChekerPathTrace && !move.isInteractable(chank))
                            continue;

                        var hLvl = move.RootUnit.StandChank.HeightLvl;
                        if (Mathf.Abs(chank.HeightLvl - hLvl) > 1)
                            continue;

                        resultTypeMap[x, y] = TypeMoveUnit.Move;
                        list.Add(chank);
                    }
                }
            }
        }

        private void ApplyMapWitoutHeight(UnitMOD move, List<Chank> list, UnitModInfo uModInfo,
            TypeMoveUnit[,] uModMap, Vector2 startCoord, ref TypeMoveUnit[,] resultTypeMap)
        {
            for (var x = 0; x < uModInfo.Width; x++)
            {
                for (var y = 0; y < uModInfo.Height; y++)
                {
                    if (uModMap[x, y] == TypeMoveUnit.Move)
                    {

                        var vX = x + (int)startCoord.x - uModInfo.Width / 2;
                        var vY = y + (int)startCoord.y - uModInfo.Height / 2;

                        if ((vX > map.Width || vY > map.Height * 2 || vX < 0 || vY < 0))
                            continue;

                        var chank = map[vX, vY];
                        if (chank == null)
                            continue;

                        if (move.UseAdditialChekerPathTrace && !move.isInteractable(chank))
                            continue;

                        var hLvl = move.RootUnit.StandChank.HeightLvl;
                        if (chank.HeightLvl > hLvl)
                            continue;

                        resultTypeMap[x, y] = TypeMoveUnit.Move;
                        list.Add(chank);
                    }
                }
            }
        }

        private List<Chank> GetZone(UnitMOD move, UnitModInfo uModInfo, TypeMoveUnit[,] uModMap, Vector2 startCoord)
        {
            List<Chank> list = new List<Chank>();
            for (var x = 0; x < uModInfo.Width; x++)
            {
                for (var y = 0; y < uModInfo.Height; y++)
                {
                    if (uModMap[x, y] == TypeMoveUnit.Move)
                    {
                        var vX = x + (int)startCoord.x - uModInfo.Width / 2;
                        var vY = y + (int)startCoord.y - uModInfo.Height / 2;

                        if ((vX > map.Width || vY > map.Height * 2 || vX < 0 || vY < 0))
                            continue;

                        var chank = map[vX, vY];
                        if (chank == null)
                            continue;
                        list.Add(chank);
                    }
                }
            }

            return list;
        }

        public void ActiveSelectorChanks(Card card)
        {
            var count = map.GetCountChanks();
            for (var i = 0; i < count; i++)
            {
                var chank = map[i];
                var container = chank.GetComponent<ChankContainerMOD>();
                var unit = card.GetUnit();

                if (container != null && container.IsFree(unit))
                    ActiveSelector(chank, ChankSelector.Free);
            }
        }

        public void ActiveTeamSelectorChanks(Card card, GameTeam team)
        {
            RecalculateChankTeams();
            var teamMap = GetTeamChanks(team);
            foreach (var chank in teamMap)
            {
                var container = chank.GetComponent<ChankContainerMOD>();
                var unit = card.GetUnit();

                if (container != null && container.IsFree(unit))
                    ActiveSelector(chank, ChankSelector.Free);
            }

            teamMap = GetTeamChanks(GameTeam.Neutral);
            foreach (var chank in teamMap)
            {
                var container = chank.GetComponent<ChankContainerMOD>();
                var unit = card.GetUnit();

                if (container != null && container.IsFree(unit))
                    ActiveSelector(chank, ChankSelector.Neutral);
            }
        }

        public void ActiveZonesSelectorChanks(Card card, GameTeam team)
        {
            var unitCard = card.GetUnit();
            var getter = unitCard.GetComponent<GetterResourceMOD>();
            if (getter == null)
                return;

            foreach (var mapContainer in map)
            {
                var chank = mapContainer.Value;
                var container = chank.ChankContainer;
                foreach (var unit in container.GetUnitSlots())
                    TryShowZone(unitCard, unit);
            }
        }

        private static void TryShowZone(Unit unitCard, KeyValuePair<int, Unit> unit)
        {
            var zone = unit.Value.GetComponent<ActionUnitZoneMOD>();
            var getterU = unit.Value.GetComponent<GetterResourceMOD>();
            if (getterU != null)
            {
                var list = getterU.GetResourcesZone();

                bool needShow = DetectedNeedShow(unitCard, list);

                if (zone != null && needShow)
                    zone.ShowZone();
            }
        }

        private static bool DetectedNeedShow(Unit unitCard, List<GetterResourceMOD.GetRes> list)
        {
            bool needShow = false;
            if (list.Count == 0)
                return needShow;

            foreach (var l in list)
            {
                if (l.UseZoneNegative)
                {
                    bool br = false;
                    foreach (var r in l.ZoneDontHave)
                    {
                        if (unitCard.nameUnit.Equals(r))
                        {
                            br = true;
                            break;
                        }
                    }
                    if (br)
                        continue;
                }

                if (l.UseZonePositive)
                    foreach (var r in l.ZoneHave)
                        if (unitCard.nameUnit.Equals(r))
                            needShow = true;
            }

            return needShow;
        }

        public void RecalculateChankTeams()
        {
            foreach (var mapContainer in map)
            {
                var chank = mapContainer.Value;
                chank.ActiveTeam = GameTeam.Neutral;
            }

            RecalculateTeamChank(GameTeam.Blue);
            RecalculateTeamChank(GameTeam.Red);
        }

        public List<Chank> GetTeamChanks(GameTeam team)
        {
            var result = new List<Chank>();
            foreach (var mapContainer in map)
            {
                var chank = mapContainer.Value;
                if (chank.ActiveTeam == team)
                    result.Add(chank);
            }

            return result;
        }

        private void RecalculateTeamChank(GameTeam team)
        {
            var list = GetGreenZone(team);
            foreach (var chank in list)
                chank.ActiveTeam = team;
        }

        private List<Chank> GetGreenZone(GameTeam team)
        {
            var minOtherTeamY = (float)map.Height * 2;
            var maxMyTeamY = (float)map.Height;
            bool activeMyTeam = false;
            bool activeOtherTeam = false;
            FindNearFarUnits(team, ref minOtherTeamY, ref maxMyTeamY, ref activeMyTeam, ref activeOtherTeam);
            var result = CalculateGreenZone(team, minOtherTeamY, maxMyTeamY, activeMyTeam, activeOtherTeam);
            result.AddRange(GetLastGreenChanks(team));
            return result;
        }

        private List<Chank> CalculateGreenZone(GameTeam team, float minOtherTeamY, float maxMyTeamY, bool activeMyTeam, bool activeOtherTeam)
        {
            var result = new List<Chank>();
            foreach (var mapContainer in map)
            {
                var chank = mapContainer.Value;
                var y = chank.GetCommonCoordinate(team).y;
                if (y == 0)
                {
                    result.Add(chank);
                    continue;
                }

                if (!activeMyTeam && !activeOtherTeam)
                {
                    if (y < (float)map.Height)
                        result.Add(chank);

                    continue;
                }

                if ((y < minOtherTeamY) && (y < maxMyTeamY))
                    result.Add(chank);
            }

            return result;
        }

        private void FindNearFarUnits(GameTeam team, ref float minOtherTeamY, ref float maxMyTeamY, ref bool activeMyTeam, ref bool activeOtherTeam)
        {
            foreach (var mapContainer in map)
            {
                var chank = mapContainer.Value;
                var container = chank.GetComponent<ChankContainerMOD>();
                foreach (var slot in container.GetUnitSlots())
                {
                    var unit = slot.Value;
                    if (unit.IsAffectZoneMap && unit.senor.Team != team)
                    {
                        var newY = chank.GetCommonCoordinate(team).y;

                        if (minOtherTeamY > newY)
                        {
                            activeOtherTeam = true;
                            minOtherTeamY = newY;
                        }
                    }
                    if (unit.IsAffectZoneMap && unit.senor.Team == team)
                    {
                        var newY = chank.GetCommonCoordinate(team).y;

                        if (maxMyTeamY < newY)
                        {
                            activeMyTeam = true;
                            maxMyTeamY = newY;
                        }
                    }
                }
            }
        }

        private List<Chank> GetLastGreenChanks(GameTeam team)
        {
            var result = new List<Chank>();
            foreach (var mapContainer in map)
            {
                var chank = mapContainer.Value;
                var container = chank.GetComponent<ChankContainerMOD>();

                if (container.IsFree(0))
                    continue;

                Unit unit;
                if (container.TryGetUnit(0, out unit))
                    if (unit.senor.Team == team)
                        result.Add(chank);
            }

            return result;
        }

        public void ActiveAimingChank(ChankSelectorMOD selectorMOD) => ActiveSelector(selectorMOD, ChankSelector.Aiming);

        public void ActiveFreeChank(ChankSelectorMOD selectorMOD) => ActiveSelector(selectorMOD, ChankSelector.Free);

        private void ActiveSelector(Chank chank, Material color, ChankSelector sel = ChankSelector.None)
        {
            if (chank == null)
                return;

            var selector = chank.GetComponent<ChankSelectorMOD>();
            if (selector != null)
            {
                selector.SwitchColor(color);
                selector.TryChangeState(sel);
            }
        }

        public void ActiveSelector(Chank chank, ChankSelector sel)
        {
            if (chank == null)
                return;

            var selector = chank.GetComponent<ChankSelectorMOD>();
            if (selector != null)
                selector.TryChangeState(sel);
        }

        private void ActiveSelector(ChankSelectorMOD selector, ChankSelector sel)
        {
            if (selector != null)
                selector.TryChangeState(sel);
        }

        private TypeMoveUnit[,] GetPawnOpenWay(TypeMoveUnit[,] uModMap, UnitMOD move)
        {
            var w = uModMap.GetLength(0);
            var h = uModMap.GetLength(1);
            var result = new TypeMoveUnit[w, h];
            result[w / 2, h / 2] = TypeMoveUnit.UnitPos;
            var startCoord = move.RootUnit.StandChank.CoordinateCurrent;

            FindWay(uModMap, ref result, w / 2, h / 2, w - 1, h - 1, startCoord, move.RootUnit.StandChank, move, false);
            return result;
        }

        private void FindWay(TypeMoveUnit[,] uModMap, ref TypeMoveUnit[,] resulrMap, int x, int y, int w, int h,
            Vector2 startCoord, Chank oldChank, UnitMOD move, bool wasCurrectHeiht)
        {
            TryFind(uModMap, resulrMap, x + 1, y, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x - 1, y, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x, y + 1, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x, y - 1, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x + 1, y + 1, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x + 1, y - 1, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x - 1, y + 1, w, h, startCoord, oldChank, move, wasCurrectHeiht);
            TryFind(uModMap, resulrMap, x - 1, y - 1, w, h, startCoord, oldChank, move, wasCurrectHeiht);
        }

        private void TryFind(TypeMoveUnit[,] uModMap, TypeMoveUnit[,] resulrMap, int x, int y, int w, int h,
            Vector2 startCoord, Chank oldChank, UnitMOD move, bool wasCurrectHeiht)
        {
            if ((x >= 0 && x <= w && y <= h && y >= 0) &&
                uModMap[x, y] == TypeMoveUnit.Move &&
                resulrMap[x, y] != TypeMoveUnit.Move)
            {
                var vX = x + (int)startCoord.x - (w + 1) / 2;
                var vY = y + (int)startCoord.y - (h + 1) / 2;

                if ((vX > map.Width || vY > map.Height * 2 || vX < 0 || vY < 0))
                    return;

                var chank = map[vX, vY];
                if (chank == null)
                    return;

                var hLvl = oldChank.HeightLvl;
                var diff = Mathf.Abs(chank.HeightLvl - hLvl);
                var isDiff = diff != 0;
                if (diff > 1)
                    return;

                if (isDiff && wasCurrectHeiht)
                    return;

                resulrMap[x, y] = TypeMoveUnit.Move;
                FindWay(uModMap, ref resulrMap, x, y, w, h, startCoord, chank, move, isDiff || wasCurrectHeiht);
            }
        }
    }
}

public enum TypeMoveUnit
{
    Free,
    Move,
    Attack,
    MoveAndAttack,
    UnitPos
}

public class ActionMask
{
    public string UnitName = "";
    public List<UnitModInfo> mods = new List<UnitModInfo>();
    public bool showMods;
    private UnitModInfo defaultMOD;

    public ActionMask() => defaultMOD = GetMOD("Default");

    public UnitModInfo GetMOD(string name)
    {
        if (mods != null && mods.Count > 0)
            foreach (var mod in mods)
                if (mod.Name.Equals(name))
                    return mod;

        return defaultMOD;
    }
}

public class UnitModInfo
{
    public string Name = "";
    public string mask = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
    public static int width = 9;
    public static int height = 9;
    public TypeMoveUnit[,] mapMask = new TypeMoveUnit[width, height];
    public bool showMask;
    public int Width { get => width; }
    public int Height { get => height; }
}

