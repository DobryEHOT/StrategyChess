using Game.CardSystem.MOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    [RequireComponent(typeof(Unit))]
    public class ChankHeightData : MonoBehaviour
    {
        public float OffSetY;
        public int HeightLevel;
        private Unit unit;

        private void Start()
        {
            unit = GetComponent<Unit>();
            var cardPref = unit.GetCardPrefab();

            var heigh = cardPref.GetComponent<SetHeightLvl>()?.GetHeightLvl();
            HeightLevel = heigh != null ? (int)heigh : HeightLevel;

            var offSet = cardPref.GetComponent<SetOffsetYforChank>()?.GetOffSet();
            OffSetY = offSet != null ? (float)offSet : OffSetY;
        }

        private void OnDestroy()
        {
            unit.StandChank.OffSetY -= OffSetY;
            unit.StandChank.HeightLvl -= HeightLevel;
            unit.TryDropPosition();
        }
    }
}
