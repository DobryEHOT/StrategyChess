using Game.MapSystems;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Units.MODs
{
    public class ActionUnitZoneMOD : UnitMOD
    {
        public string NameMOD = "RadiusZone";

        protected override void InitUnitMOD()
        {
            ModsName = NameMOD;
        }

        public void ShowZone()
        {
            ChanksMoving = GetListChanks();
            if (ChanksMoving != null && ChanksMoving.Count > 0)
            {
                foreach (var chank in ChanksMoving)
                {
                    if (chank == null)
                        continue;

                    if (isInteractable(chank))
                    {
                        Singleton<MapSystem>.MainSingleton.ChanksController.ActiveChank(chank, GetColorChank());
                    }
                }
            }
        }

        public List<Unit> GetUnitsInZone()
        {
            var newChanksMoving = new List<Unit>();

            ChanksMoving = GetListChanks();
            if (ChanksMoving != null && ChanksMoving.Count > 0)
            {
                foreach (var chank in ChanksMoving)
                {
                    if (chank == null)
                        continue;

                    if (isInteractable(chank))
                    {
                        var values = chank.ChankContainer.GetUnitSlots().Values;
                        newChanksMoving.AddRange(values);
                    }
                }
            }

            return newChanksMoving;
        }

        public override List<Chank> GetListChanks() => Singleton<MapSystem>.MainSingleton.ChanksController.GetZoneChanks(this); // ChanksMoving
    }
}