using Assets.Scripts.SelectorSpace;
using Game.Singleton;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class VizorData : MonoBehaviour, ISelectItem
    {
        [SerializeField] protected List<VizorItem> vizorItems = new List<VizorItem>();
        [SerializeField] private Vector3 offSet;
        private GameObject vizorObj;
        private Vizor vizor;
        private bool active = false;
        private LochalClientInformation lochalClient;
        private Camera lochalPlayerCamera;
        private Player lochalPlayer;
        private float secndsWait = 0.7f;
        private WaitForSeconds waitForSeconds;

        public virtual void Start()
        {
            InitClient();
            InitVizor();
            waitForSeconds = new WaitForSeconds(secndsWait);
        }

        private void InitVizor()
        {
            vizorObj = Singleton<MainScreen>.MainSingleton.GetChankVizor();
            vizor = vizorObj.GetComponent<Vizor>();
            vizorObj.SetActive(false);
        }

        private void InitClient()
        {
            lochalClient = Singleton<LochalClientInformation>.MainSingleton;
            lochalPlayerCamera = lochalClient.Info.player.MainCamera;
            lochalPlayer = lochalClient.Info.player;
        }

        public virtual void OnUnAimSelectItem(SelectorItems selector)
        {
            if (vizorObj == null)
                return;

            active = false;
            vizorObj.SetActive(false);
        }

        public virtual void OnAimSelectItem(SelectorItems selector)
        {
            if (!selector.ActiveVizorAlways || vizorObj == null || vizor == null)
                return;

            if (!selector.ActiveVizorAlways)
                return;

            vizor.SetVizorItems(vizorItems);
            Vector3 posOnScreen = GetVizorScreenPosition();
            active = true;
            StartCoroutine(TimerToActive());
            vizorObj.transform.position = posOnScreen + vizorObj.transform.forward;
        }

        private Vector3 GetVizorScreenPosition()
        {
            Vector3 posOnScreen = Vector3.zero;
            if (lochalPlayerCamera != null)
            {
                Vector3 newOffSet = GetCurrectOffset();
                var chankPos = transform.position;
                posOnScreen = lochalPlayerCamera.WorldToScreenPoint(chankPos);
                var resCamera = lochalPlayer.ResourcesCamera;
                if (resCamera != null)
                    posOnScreen = resCamera.ScreenToWorldPoint(posOnScreen) + newOffSet;
            }

            return posOnScreen;
        }

        private Vector3 GetCurrectOffset()
        {
            var mousePosition = Input.mousePosition;
            var x = Screen.width;
            var y = Screen.height;
            var newOffSet = offSet;
            if (mousePosition.x > x / 2)
                newOffSet.x *= -1;

            if (mousePosition.y < y / 2)
                newOffSet.y *= -1;
            return newOffSet;
        }

        public virtual void OnSelectItem(SelectorItems selector) { }

        public virtual void OnUnselectItem(SelectorItems selector) { }

        private IEnumerator TimerToActive()
        {
            yield return waitForSeconds;

            if (active)
                vizorObj.SetActive(true);
        }
    }
}
