using Assets.Scripts.UnitsSystem.Interfaces;
using Game.Baff;
using Game.MapSystems;
using Game.MapSystems.MOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    [RequireComponent(typeof(Unit))]
    public class BaffUnitMOD : UnitMOD, IRaundAction
    {
        public List<BaffObj> templateBaffs = new List<BaffObj>();
        private Unit rootUnit;
        public void DoAfterAction(Chank chank) { }

        public void DoBeforeAction() { }

        public void DoUpdateAction(Chank chank) { }

        protected override void InitUnitMOD()
        {
            rootUnit = GetComponent<Unit>();
        }

        public void AddBaff(params BaffObj[] baffs)
        {
            templateBaffs.AddRange(baffs);
        }

        public void DoBaffActions()
        {
            var list = new List<BaffObj>();
            if (templateBaffs.Count != 0)
            {
                foreach (var baff in templateBaffs)
                {
                    if (baff != null)
                    {
                        baff.DoAction(rootUnit);
                        if (baff.isInfinity)
                            list.Add(baff);
                    }
                }
            }

            templateBaffs = list;
        }

        public void OnStartRaund()
        {

        }

        public void OnEndRaund()
        {
            DoBaffActions();
        }

        public override void OnMove(Chank chank)
        {
            var units = chank.ChankContainer.GetUnitSlots();
            if (units.Count > 0)
            {
                foreach (var u in units)
                {
                    if (u.Value != null)
                    {
                        var mod = u.Value.GetComponent<GetterBaffUnitMOD>();
                        if (mod != null)
                            mod.InsertBaffs(this);
                    }
                }
            }
        }
    }
}