using Game.MapSystems;
using Game.Units;
using UnityEngine;

namespace Game.Baff
{
    [CreateAssetMenu(fileName = "BaffWaitRaund", menuName = "ScriptableObjects/Baff/BaffWaitRaund", order = 1)]
    public class BaffWaitRaund : BaffObj
    {
        public int WaitRaundCount = 0;
        public override void DoAction(Unit objectPawn)
        {
            var ticker = objectPawn.GetComponent<RaundTickerUnitMOD>();
            if (ticker != null)
                ticker.MoveMODCount -= WaitRaundCount;
        }
    }
}