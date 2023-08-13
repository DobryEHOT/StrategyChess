using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapSystems.MOD
{
    [RequireComponent(typeof(Chank))]
    public abstract class ChankMOD : MonoBehaviour
    {
        protected Chank RootChank { get; set; }
        protected virtual void Awake() => RootChank = GetComponent<Chank>();
    }
}
