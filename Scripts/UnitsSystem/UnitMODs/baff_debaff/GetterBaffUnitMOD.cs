using Game.Units;
using Game.Units.MODs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Baff
{
    [RequireComponent(typeof(Unit))]
    public class GetterBaffUnitMOD : MonoBehaviour
    {
        [SerializeField]
        private List<BaffObj> getBaffs = new List<BaffObj>();

        [SerializeField]
        private TypeWorkGetter typeGetter;

        public List<BaffObj> GetBaff() => getBaffs;

        public void InsertBaffs(BaffUnitMOD unitBaff)
        {
            unitBaff.AddBaff(getBaffs.ToArray());
        }
    }

    public enum TypeWorkGetter
    { 
        RaundStart,
        RaundEnd,
        RaundContinue
    }
}
