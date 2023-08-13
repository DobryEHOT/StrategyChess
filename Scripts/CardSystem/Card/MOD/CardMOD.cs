using Game.MapSystems;
using UnityEngine;

namespace Game.CardSystem.MOD
{
    [RequireComponent(typeof(Card))]

    public abstract class CardMOD : MonoBehaviour
    {
        protected Card rootCard;
        void Start() 
        {
            rootCard = GetComponent<Card>();
            rootCard.AddActionOnUse(OnUse);
            rootCard.AddActionOnTake(OnTake);
            OnStart();
        }

        protected abstract void OnStart();
        protected abstract void OnUse(Chank chank, Player player);
        protected abstract void OnTake();
    }
}