using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Singleton;
using UnityEngine.Rendering;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using System.IO;

namespace Game
{
    public class GameSettings : Singleton<GameSettings>
    {
        private SaveSystem saveSystem;

        [SerializeField] private List<AudioMixerGroup> musicsGroup = new List<AudioMixerGroup>();
        [SerializeField] private List<AudioMixerGroup> soundsGroup = new List<AudioMixerGroup>();

        private string nameSettingsGlobal = "QualitySettings-Global";
        private string nameSettingsResolution = "QualitySettings-Resolution";
        private string nameSettingsMusicVolume = "QualitySettings-MusicVolume";
        private string nameSettingsSoundVolume = "QualitySettings-SoundVolume";
        private string nameSettingsGlobalVolume = "QualitySettings-GlobalVolume";

        void Awake()
        {
            base.Inicialize();
            if (Application.platform == RuntimePlatform.Android)
                Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;// Screen.currentResolution.refreshRate;
        }

        private void Start()
        {
            saveSystem = Singleton<SaveSystem>.MainSingleton;
            FirstLoadData();
        }

        private void FirstLoadData()
        {
            float value;
            if (TryGetDataFloat(nameSettingsGlobal, out value))
                SetQualityGraphics(value);

            if (TryGetDataFloat(nameSettingsResolution, out value))
                SetResolution(value);

            if (TryGetDataFloat(nameSettingsMusicVolume, out value))
                SetMusicVolume(value);

            if (TryGetDataFloat(nameSettingsSoundVolume, out value))
                SetSoundVolume(value);

            if (TryGetDataFloat(nameSettingsGlobalVolume, out value))
                SetGlobalVolume(value);
        }

        public void SetQualityGraphics(float index)
        {
            QualitySettings.SetQualityLevel((int)index);
            saveSystem.TryRewriteData(nameSettingsGlobal, index.ToString());

            float value;
            if (TryGetDataFloat(nameSettingsResolution, out value))
                SetResolution(value);

            saveSystem.SaveData();
        }

        public void SetResolution(float index)
        {
            var clearVar = index / 100f;

            saveSystem.TryRewriteData(nameSettingsResolution, index.ToString());
            
            var renderPip = QualitySettings.renderPipeline;
            if (renderPip is UniversalRenderPipelineAsset univ)
                univ.renderScale = clearVar;

            saveSystem.SaveData();
        }

        public void SetMusicVolume(float index)
        {
            saveSystem.TryRewriteData(nameSettingsMusicVolume, index.ToString());
            SetVolume(musicsGroup, index);

            float value;
            if (TryGetDataFloat(nameSettingsGlobalVolume, out value))
                SetGlobalVolume(value);

            saveSystem.SaveData();
        }

        public void SetSoundVolume(float index)
        {
            saveSystem.TryRewriteData(nameSettingsSoundVolume, index.ToString());
            SetVolume(soundsGroup, index);

            float value;
            if (TryGetDataFloat(nameSettingsGlobalVolume, out value))
                SetGlobalVolume(value);

            saveSystem.SaveData();
        }

        public void SetGlobalVolume(float index)
        {
            saveSystem.TryRewriteData(nameSettingsGlobalVolume, index.ToString());

            ResetGlobalVolume(nameSettingsSoundVolume, soundsGroup, index);
            ResetGlobalVolume(nameSettingsMusicVolume, musicsGroup, index);

            saveSystem.SaveData();
        }

        private void ResetGlobalVolume(string dataName, List<AudioMixerGroup> group, float index)
        {
            string value;
            if (saveSystem.TryGetDataToName(dataName, out value))
            {
                float parse;
                if (float.TryParse(value, out parse))
                    SetVolume(group, index * parse);
            }
        }

        private bool TryGetDataFloat(string dataName, out float index)
        {
            string value;
            if (saveSystem.TryGetDataToName(dataName, out value))
                if (float.TryParse(value, out index))
                    return true;

            index = 0;
            return false;
        }

        private void SetVolume(List<AudioMixerGroup> groups, float index)
        {
            foreach (var group in groups)
                group.audioMixer.SetFloat("SoundVolume", Mathf.Log10(index) * 20);
        }
    }
}
