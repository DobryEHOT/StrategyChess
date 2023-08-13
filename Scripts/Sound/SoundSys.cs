using Game.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sound
{
    [RequireComponent(typeof(AudioClip))]
    public class SoundSys : Singleton<SoundSys>
    {
        [SerializeField]
        private List<SoundInfo> sounds = new List<SoundInfo>();
        private Dictionary<string, AudioClip> audioDiction = new Dictionary<string, AudioClip>();
        private AudioSource source;

        void Awake()
        {
            Inicialize();
            InitAudio();
        }

        private void InitAudio()
        {
            foreach (var so in sounds)
                audioDiction.Add(so.name, so.clip);

            source = GetComponent<AudioSource>();
        }

        public bool TryPlaySound(string nameClip, Vector3 globalPosition = new Vector3())
        {
            AudioClip clip;
            if (audioDiction.TryGetValue(nameClip, out clip))
            {
                source.transform.position = globalPosition;
                source.PlayOneShot(clip);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public struct SoundInfo
    {
        [SerializeField]
        public string name;
        [SerializeField]
        public AudioClip clip;
    }
}
