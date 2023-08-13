using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class GameSettingsConnector : MonoBehaviour
    {
        private GameSettings settings;
        void Awake()
        {
            settings = Singleton<GameSettings>.MainSingleton;
        }

        public void SetQualityGraphics(float index) => settings?.SetQualityGraphics(index);
        public void SetResolution(float index) => settings?.SetResolution(index);
        public void SetMusicVolume(float index) => settings?.SetMusicVolume(index);
        public void SetSoundVolume(float index) => settings?.SetSoundVolume(index);
        public void SetGlobalVolume(float index) => settings?.SetGlobalVolume(index);

    }
}
