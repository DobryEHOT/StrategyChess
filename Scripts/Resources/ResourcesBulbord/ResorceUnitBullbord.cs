using Game.CameraController;
using TMPro;
using UnityEngine;

namespace Game.CardSystem
{
    public class ResorceUnitBullbord : MonoBehaviour
    {
        public ResourceType ResourceType;
        [SerializeField] private TextMeshProUGUI text;

        public void SetValue(int value) => text.text = value.ToString();
    }
}

