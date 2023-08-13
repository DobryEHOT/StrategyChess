using Game.MapSystems;
using Game.MapSystems.MOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.CardSystem.MOD
{
    public class SetAlwaysBusy : CardMOD
    {
        [SerializeField] private bool alwaysBusy = false;

        protected override void OnStart() { }

        protected override void OnTake() { }

        protected override void OnUse(Chank chank, Player player) => chank.StartCoroutine(DoHeightLvl(chank));

        IEnumerator DoHeightLvl(Chank chank)
        {
            var waiter = new WaitForFixedUpdate();
            yield return waiter;
            yield return waiter;
            yield return waiter;
            yield return waiter;
            yield return waiter;

            chank.GetComponent<ChankContainerMOD>().SetAlwaysBusy(alwaysBusy);
        }
    }
}