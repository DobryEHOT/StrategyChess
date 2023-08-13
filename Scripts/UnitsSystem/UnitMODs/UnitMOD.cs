
using Game.MapSystems;
using Game.Singleton;
using Game.Sound;
using Game.Units;
using Game.Units.MOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Units.MODs
{
    [RequireComponent(typeof(Unit))]
    public abstract class UnitMOD : MonoBehaviour, IMovmentMOD
    {
        [SerializeField] private Color colorChanks;
        [SerializeField] private Material materialChanks;
        protected string ModsName = "Default";
        protected ActionMask action;
        protected UnitModInfo info;
        public List<Chank> ChanksMoving { get; protected set; } = new List<Chank>();
        public Unit RootUnit { get; protected set; }
        public virtual bool UseAdditialChekerPathTrace { get; } = true;
        [SerializeField] [Range(0, 10)] private int actionPriority = 0;
        protected string nameSound = "";
        private SoundSys soundSys;

        private void Awake() => RootUnit = GetComponent<Unit>();

        private void Start() => soundSys = Singleton<SoundSys>.MainSingleton;

        public void SetMaskActions(ActionMask action)
        {
            InitUnitMOD();
            this.action = action;
            if (action != null)
                info = action.GetMOD(ModsName);
        }

        public ActionMask GetActionMask() => action;

        public UnitModInfo GetUnitModInfo() => info;

        public virtual void OnMove(Chank chank) => PlaySound();

        public virtual bool isInteractable(Chank chank) => true;

        public Material GetColorChank() => materialChanks;

        public virtual List<Chank> GetListChanks() => Singleton<MapSystem>.MainSingleton.ChanksController.GetMoveChanks(this);

        public int GetPriority() => actionPriority;

        protected abstract void InitUnitMOD();

        protected void PlaySound()
        {
            soundSys.TryPlaySound(nameSound, transform.position);
            RootUnit.TryDropPosition();
        }
    }
}
