using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapSystems.MOD
{
    [RequireComponent(typeof(ChankContainerMOD))]
    public class ChankSelectorMOD : ChankMOD
    {
        [SerializeField] private Material freeSlot;
        [SerializeField] private Material aimingSlot;
        [SerializeField] private Material moveSlot;
        [SerializeField] private Material attackSlot;
        [SerializeField] private Material NeitralSlot;

        [SerializeField] private Material attackShowerSlot;
        [SerializeField] private Material attackDistanceShowerSlot;

        [Space]
        [SerializeField] private GameObject indificator;

        private ChankContainerMOD containerMOD;
        private Renderer render;
        private BoxCollider boxCollider;

        private ChankSelector chankSelector = ChankSelector.Disable;

        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            render = indificator.GetComponent<Renderer>();
            containerMOD = GetComponent<ChankContainerMOD>();
        }

        //public void ShowIsFree()
        //{


        //    render.material = freeSlot;
        //}

        //public void ShowIsAiming()
        //{
        //    render.material = aimingSlot;
        //}

        //public void ShowIsUnitMove()
        //{
        //    render.material = moveSlot;
        //}

        //public void ShowIsUnitAttack()
        //{
        //    render.material = attackSlot;
        //}

        public void SwitchColor(Material color)
        {
            //render.material.color = color;
            if (color == null)
                render.material = freeSlot;
            else
                render.material = color;
        }

        public void TryChangeState(ChankSelector newSelector)
        {
            indificator.transform.position = new Vector3(indificator.transform.position.x, RootChank.OffSetY, indificator.transform.position.z);
            boxCollider.center = new Vector3(boxCollider.center.x, RootChank.OffSetY, boxCollider.center.z);

            if (chankSelector == newSelector)
                return;

            if (newSelector == ChankSelector.Disable)
            {
                indificator.SetActive(false);
                //render.material = freeSlot;
                //render.material.color = Color.white;

            }
            else
            {
                indificator.SetActive(true);

                if (newSelector == ChankSelector.Neutral)
                    render.material = NeitralSlot;
                if (newSelector == ChankSelector.Free)
                    render.material = freeSlot;
                if (newSelector == ChankSelector.AttackDistanceShower)
                    render.material = attackDistanceShowerSlot;
                if (newSelector == ChankSelector.AttackShower)
                    render.material = attackShowerSlot;
                else if (newSelector == ChankSelector.Aiming)
                    render.material = aimingSlot;
                //else if (newSelector == ChankSelector.Attack)
                //    render.material.color = attackSlot.color;


                /*
                if (newSelector == ChankSelector.Aiming)
                    render.material = aimingSlot;
                if (newSelector == ChankSelector.Free)
                    render.material = freeSlot;
                if (newSelector == ChankSelector.Move)
                    render.material = moveSlot;
                if (newSelector == ChankSelector.Attack)
                    render.material = attackSlot;
                */
            }

            chankSelector = newSelector;
        }
    }
    public enum ChankSelector
    {
        Disable,
        Free,
        Aiming,
        Move,
        Attack,
        Neutral,
        None,
        AttackShower,
        AttackDistanceShower,
        Zone
    }
}