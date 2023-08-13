using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sound
{

    public class PlaySoundOnDestroy : MonoBehaviour
    {
        [SerializeField] private string nameSound;
        [SerializeField] private bool usePosion = true;
        private SoundSys soundSys;

        private void Start()
        {
            soundSys = Singleton<SoundSys>.MainSingleton;
        }

        private void OnDestroy()
        {
            if (soundSys != null)
                if (usePosion)
                    soundSys.TryPlaySound(nameSound, transform.position);
                else
                    soundSys.TryPlaySound(nameSound);
        }
    }
}
