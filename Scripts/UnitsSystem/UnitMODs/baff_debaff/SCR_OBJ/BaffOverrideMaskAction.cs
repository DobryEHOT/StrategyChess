using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Baff
{
    [CreateAssetMenu(fileName = "BaffOverrideMoveMap", menuName = "ScriptableObjects/Baff/BaffOverrideMoveMap", order = 1)]

    public class BaffOverrideMaskAction : BaffObj
    {
        public string overrideNamePawnMaskAction  ="Default";
        public override void DoAction(Unit objectPawn)
        {
            objectPawn.OverrideActionMaskToName(overrideNamePawnMaskAction);
        }
    }
}
