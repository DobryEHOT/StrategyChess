using System.Collections;
using System.Collections.Generic;
using Game.MapSystems;
using UnityEngine;
namespace Game.CardSystem.MOD
{
    public class SetHeightLvl : CardMOD
    {
        [SerializeField] private int setHeightLvl = 0;

        public int GetHeightLvl() => setHeightLvl;
        protected override void OnStart() { }

        protected override void OnTake() { }

        protected override void OnUse(Chank chank, Player player)
        {
            chank.StartCoroutine(DoHeightLvl(chank));
        }

        IEnumerator DoHeightLvl(Chank chank)
        {
            var waiter = new WaitForFixedUpdate();
            yield return waiter;
            yield return waiter;
            yield return waiter;
            yield return waiter;
            yield return waiter;

            chank.HeightLvl += setHeightLvl;
        }
    }
}