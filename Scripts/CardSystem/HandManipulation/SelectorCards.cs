//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.CardSystem.HandManipulation
//{
//    [RequireComponent(typeof(Player))]
//    public class SelectorCards : MonoBehaviour
//    {
//        [SerializeField] private Camera mainCamera;
//        [SerializeField] private LayerMask cardLayer;

//        private GameObject takingCard;
//        private CardMover selectedCard;
//        void Start()
//        {
//            if (!mainCamera)
//                throw new System.Exception("You need set camera into SelectorCards!!!");
//        }

//        void Update()
//        {
//            TrySelectCard();
//        }

//        private void TrySelectCard()
//        {
//            if (Input.GetMouseButton(0))
//                return;

//            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
//            ray.direction *= 2f;

//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit, 100f, cardLayer))
//            {
//                hit.collider.gameObject.GetComponent<CardMover>().DoSelected();
//            }
//        }
//    }
//}
