using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.CardSystem.MOD
{
    public class SetOffsetYforChank : CardMOD
    {
        [SerializeField] private float OffsetYforChank = 0;

        public float GetOffSet() => OffsetYforChank;

        protected override void OnStart() { }

        protected override void OnTake() { }

        protected override void OnUse(Chank chank, Player player)
        {
            chank.StartCoroutine(DoSetOffset(chank));
        }

        IEnumerator DoSetOffset(Chank chank)
        {
            var waiter = new WaitForFixedUpdate();
            yield return waiter;
            yield return waiter;
            yield return waiter;
            yield return waiter;
            yield return waiter;

            chank.OffSetY += OffsetYforChank;
        }
    }
}