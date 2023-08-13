using Game.MapSystems;
using Game.MapSystems.Generator;
using Game.Singleton;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    [RequireComponent(typeof(Unit))]
    public class RotatorMOD : UnitMOD
    {
        [SerializeField] private bool recalculateRotaionAroundPawns = true;
        [SerializeField] [Range(0, 10)] private int id;
        [SerializeField] private GameObject upObject;
        [SerializeField] private GameObject downObject;
        [SerializeField] private GameObject rightObject;
        [SerializeField] private GameObject leftObject;
        private GameObject activeObject;

        public int ID { get => id; }
        protected override void InitUnitMOD() { }

        private void Start()
        {
            var map = Singleton<MapSystem>.MainSingleton.ChanksController.GetMap();
            var coordinate = RootUnit.StandChank.CoordinateCurrent;

            if (recalculateRotaionAroundPawns)
                Rotate(map, coordinate);

            activeObject = upObject;
            activeObject.SetActive(true);

            RecalculateRotation();
        }

        private void Rotate(GameMap map, Vector2 coordinate)
        {
            var list = new List<Chank>();
            list.Add(map[(int)coordinate.x + 1, (int)coordinate.y]);
            list.Add(map[(int)coordinate.x - 1, (int)coordinate.y]);
            list.Add(map[(int)coordinate.x, (int)coordinate.y + 1]);
            list.Add(map[(int)coordinate.x, (int)coordinate.y - 1]);

            foreach (var item in list)
            {
                if (item != null)
                {
                    var unit = GetUnit(map, item.CoordinateCurrent);
                    if (unit != null)
                    {
                        var rotator = unit.GetComponent<RotatorMOD>();
                        if (rotator != null)
                        {
                            rotator.RecalculateRotation();
                        }
                    }
                }
            }
        }

        public void RecalculateRotation()
        {
            var map = Singleton<MapSystem>.MainSingleton.ChanksController.GetMap();
            var coordinate = RootUnit.StandChank.CoordinateCurrent;

            if (TrySwitchRotatePrefab(map, coordinate + new Vector2(0, 1), upObject))
                return;
            if (TrySwitchRotatePrefab(map, coordinate + new Vector2(0, -1), downObject))
                return;
            if (TrySwitchRotatePrefab(map, coordinate + new Vector2(1, 0), rightObject))
                return;
            if (TrySwitchRotatePrefab(map, coordinate + new Vector2(-1, 0), leftObject))
                return;
        }

        private bool TrySwitchRotatePrefab(GameMap map, Vector2 coordinate, GameObject rotatePrefab)
        {
            var unit = GetUnit(map, coordinate);
            if (unit != null)
            {
                var rotator = unit.GetComponent<RotatorMOD>();
                if (rotator != null)
                {
                    if (rotator.ID == ID)
                    {
                        if (activeObject != null)
                            activeObject.SetActive(false);

                        activeObject = rotatePrefab;
                        activeObject.SetActive(true);
                        return true;
                    }
                }
            }

            return false;
        }

        private Unit GetUnit(GameMap map, Vector2 coordinate)
        {
            var chank = map[(int)coordinate.x, (int)coordinate.y];
            if (chank == null)
                return null;

            Unit u;
            if (chank.ChankContainer.TryGetUnit(RootUnit.ChankQueue, out u))
                return u;

            return null;
        }
    }
}
