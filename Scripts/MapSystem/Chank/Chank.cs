using Game.MapSystems.Enums;
using Game.MapSystems.MOD;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapSystems
{
    [RequireComponent(typeof(ChankContainerMOD))]
    public class Chank : MonoBehaviour
    {
        [SerializeField] private MeshRenderer defaultMeshRender;

        public int Number { get; private set; }
        public ChankContainerMOD ChankContainer { get; private set; }
        public GameTeam Team { get; private set; }
        public Vector2 Coordinate { get; private set; }
        public Vector2 CoordinateSecond { get => new Vector2(width - Coordinate.x - 1, height + height - 1 - Coordinate.y); }
        public Vector2 CoordinateCurrent { get; set; }
        public Vector2 CoordinateCurrentInvert { get => new Vector2(width - CoordinateCurrent.x - 1, height + height - 1 - CoordinateCurrent.y); }
        public float OffSetY = 0;
        public float HeightLvl = 0;
        public GameTeam ActiveTeam = GameTeam.Neutral;
        private int width;
        private int height;

        private void Start() => ChankContainer = GetComponent<ChankContainerMOD>();

        public int GetFirstNumber() => Number;

        public int GetSecondNumber() => 2 * (height * width) - Number;

        public int GetCommonNumber() => Team == GameTeam.Blue ? GetFirstNumber() : GetSecondNumber();

        public Vector2 GetCommonCoordinate(GameTeam team) => team == GameTeam.Blue ? CoordinateCurrent : CoordinateCurrentInvert;

        public void SetActiveMeshRenderChank(bool active) => defaultMeshRender.enabled = active;

        public void SetSpawnInfo(int i, GameTeam gameTeam, int width, int height, Vector2 coordinate)
        {
            Coordinate = coordinate;
            Number = i;
            Team = gameTeam;
            this.width = width;
            this.height = height;
        }

        public void SetPawnToChank(GameObject pawn)
        {
            if (pawn != null)
                pawn.transform.SetParent(transform);
            else
                return;

            Unit unit = pawn.GetComponent<Unit>();

            var container = GetComponent<ChankContainerMOD>();

            if (container != null)
                if (pawn != null)
                    container.TrySetUnit(unit);
        }

        public void ClearPawnToChank(GameObject pawn)
        {
            if (pawn == null)
                return;

            var container = GetComponent<ChankContainerMOD>();
            var unit = pawn.GetComponent<Unit>();

            if (pawn != null && container != null && unit != null)
                container.ClearUnitSlot(unit.ChankQueue);
        }

        public void ClearPawn(int queueLevel)
        {
            var container = GetComponent<ChankContainerMOD>();
            if (container != null)
                container.ClearUnitSlotWithDestroy(queueLevel);
        }
    }
}
