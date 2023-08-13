using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Game.CardSystem;
using Game;

namespace Assets.Scripts.SelectorSpace
{
    public class SelectorItems : MonoBehaviour
    {
        public bool ActiveVizor { get; set; } = true;
        public bool ActiveVizorAlways { get; set; } = true;
        public Player Player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask selectItemLayer;
        private LayerMask maskChank;
        private ISelectItem lastAimedItem;

        void Start()
        {
            if (!mainCamera)
                throw new System.Exception("You need set camera into SelectorCards!!!");

            maskChank = LayerMask.GetMask("Chank");
        }

        void Update()
        {
            TrySelectCard();
        }

        public bool TrySelect(out RaycastHit hit)
        {
            Ray ray = GetCameraRay();
            if (Physics.Raycast(ray, out hit, 100, maskChank) && hit.collider.tag != "HandProtection")
                return true;
            else
                return false;
        }

        public Vector3 GetPositionCursoure()
        {
            var a = mainCamera.ScreenPointToRay(Input.mousePosition);
            return a.origin + (a.direction * 6f);
        }

        private Ray GetCameraRay()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            ray.direction *= 2f;
            Debug.DrawRay(ray.origin, ray.direction);

            return ray;
        }

        private void TrySelectCard()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            ray.direction *= 2f;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, selectItemLayer))
            {
                var items = hit.collider.gameObject.GetComponents<ISelectItem>();

                if (items == null || (items != null && items.Length == 0))
                    UnAiming();

                DoSelect(items);
            }
            else
            {
                UnAiming();
            }
        }

        private void DoSelect(ISelectItem[] items)
        {
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    Aiming(item);

                    if (Input.GetMouseButtonDown(0))
                        item.OnSelectItem(this);

                    if (Input.GetMouseButtonUp(0))
                        item.OnUnselectItem(this);
                }

                return;
            }
        }

        private void Aiming(ISelectItem item)
        {
            if (ActiveVizor && !item.Equals(lastAimedItem))
            {
                UnAiming();
                item.OnAimSelectItem(this);
                lastAimedItem = item;
            }
        }

        private void UnAiming()
        {
            if (ActiveVizor && lastAimedItem != null)
            {
                lastAimedItem.OnUnAimSelectItem(this);
                lastAimedItem = null;
            }
        }
    }
}
