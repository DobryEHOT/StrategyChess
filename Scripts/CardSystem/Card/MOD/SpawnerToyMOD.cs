using Game.MapSystems;
using Game.MapSystems.MOD;
using Game.Units;
using UnityEngine;

namespace Game.CardSystem.MOD
{
    [RequireComponent(typeof(Card))]
    public class SpawnerToyMOD : CardMOD
    {
        [SerializeField] private GameObject prefabToy;
        [SerializeField] private bool rotateOnSpawn = false;
        private PrepositionObjController controller;

        protected override void OnStart() => controller = GetComponent<PrepositionObjController>();

        protected override void OnTake() { }

        protected override void OnUse(Chank chank, Player player)
        {
            var pawn = SpawnPawn(prefabToy, GetDefaultPosition(chank), GetDefaultRotation(), player, chank);
            chank.SetPawnToChank(pawn);
        }

        public Unit GetUnit()
        {
            if (prefabToy == null)
                return null;

            return prefabToy.GetComponent<Unit>();
        }

        public Vector3 GetDefaultPosition(Chank chank)
        {
            if (controller == null)
                controller = GetComponent<PrepositionObjController>();

            if (controller == null || chank == null)
                return Vector3.zero;

            return chank.transform.position + controller.offSetPrePosition + Vector3.up * chank.OffSetY;
        }

        public Quaternion GetDefaultRotation() => Quaternion.Euler(controller.offSetPreRotation);

        private GameObject SpawnPawn(GameObject toy, Vector3 position, Quaternion rotation, Player player, Chank chank)
        {
            GameObject pawn = InitPawn(toy, position, rotation, chank);
            InitUnit(player, pawn);
            return pawn;
        }

        private Unit InitUnit(Player player, GameObject pawn)
        {
            var unit = pawn.GetComponent<Unit>();
            if (unit != null)
            {
                unit.senor = player;
                var container = GetComponent<ChankContainerMOD>();

                if (container != null)
                    container.TrySetUnit(unit);

                if (rotateOnSpawn)
                {
                    var angle = pawn.transform.eulerAngles;
                    pawn.transform.rotation = Quaternion.Euler(angle.x, player.transform.eulerAngles.y, angle.z);
                }
            }

            return unit;
        }

        private static GameObject InitPawn(GameObject toy, Vector3 position, Quaternion rotation, Chank chank)
        {
            if (!toy)
                throw new System.Exception("Toy is null!!!");

            var pawn = Instantiate(toy, position, rotation);
            pawn.GetComponent<Unit>().SwitchStandChank(chank);
            return pawn;
        }
    }
}