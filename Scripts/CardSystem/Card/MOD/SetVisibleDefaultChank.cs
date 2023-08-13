using Game.MapSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CardSystem.MOD
{
    public class SetVisibleDefaultChank : CardMOD
    {
        [SerializeField] private bool isVisible = false;
        protected override void OnStart() { }

        protected override void OnTake() { }

        protected override void OnUse(Chank chank, Player player) => chank.SetActiveMeshRenderChank(isVisible);
    }
}
