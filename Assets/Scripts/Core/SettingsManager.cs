using Astutos.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

namespace CON.Core
{
    public class SettingsManager : MonoBehaviour,ISaveable
    {
        [SerializeField] AudioSetting gUIAudio;
        [SerializeField] AudioSetting playerAudio;
        [SerializeField] AudioSetting machineAudio;
        [SerializeField] AudioSetting backgroundMusicAudio;

        SavingSystemJson jsonSaving;

        const string settingsFileName = "settings";

        private void Start()
        {
            jsonSaving = FindObjectOfType<SavingSystemJson>();
        }


        public void Save()
        {
             float guiVolume;
             float playerVolume;
             float machineVolume;
             float backgroundVolume;

            gUIAudio.audioMixer.GetFloat("volume", out guiVolume);
            playerAudio.audioMixer.GetFloat("volume", out playerVolume);
            machineAudio.audioMixer.GetFloat("volume", out machineVolume);
            backgroundMusicAudio.audioMixer.GetFloat("volume", out backgroundVolume);

            SavedSettings savedSettings = new SavedSettings(guiVolume, playerVolume, machineVolume, backgroundVolume);
            jsonSaving.Save(settingsFileName, savedSettings);
        }

        public void Load()
        {
            if(jsonSaving == null) jsonSaving = FindObjectOfType<SavingSystemJson>();
            SavedSettings savedSettings = JsonUtility.FromJson<SavedSettings>(jsonSaving.Load(settingsFileName));

            ChangeGUIVolume(savedSettings.guiVolume);
            gUIAudio.audioSlider.value = savedSettings.guiVolume;

            ChangePlayerVolume(savedSettings.playerVolume);
            playerAudio.audioSlider.value = savedSettings.playerVolume;

            ChangeMachineVolume(savedSettings.machineVolume);
            machineAudio.audioSlider.value = savedSettings.machineVolume;

            ChangeBackgroundVolume(savedSettings.backgroundVolume);
            backgroundMusicAudio.audioSlider.value = savedSettings.backgroundVolume;
        }

        // -----------   AUDIO -------------

        public void ResetAllVolumes()
        {
            ChangeGUIVolume(0);
            gUIAudio.audioSlider.value = 0;

            ChangePlayerVolume(0);
            playerAudio.audioSlider.value = 0;

            ChangeMachineVolume(0);
            machineAudio.audioSlider.value = 0;

            ChangeBackgroundVolume(0);
            backgroundMusicAudio.audioSlider.value = 0;
        }

        public void ChangeGUIVolume(float volume)
        {
            ChangeVolume(gUIAudio, volume);
        }
        public void ChangePlayerVolume(float volume)
        {
            ChangeVolume(playerAudio, volume);
        }
        public void ChangeMachineVolume(float volume)
        {
            ChangeVolume(machineAudio, volume);
        }
        public void ChangeBackgroundVolume(float volume)
        {
            ChangeVolume(backgroundMusicAudio, volume);
        }
        private void ChangeVolume(AudioSetting audioMixer, float volume)
        {
            
            if (Mathf.Approximately(-40f,volume)) volume = -80;
            audioMixer.audioMixer.SetFloat("volume", volume);
        }

        public object CaptureState()
        {
            Save();
            return null;
        }

        public void RestoreState(object state)
        {
            Load();
        }

        [System.Serializable]
        public class AudioSetting
        {
            public AudioMixer audioMixer;
            public Slider audioSlider;
        }

        [System.Serializable]
        private class SavedSettings
        {
            public float guiVolume;
            public float playerVolume;
            public float machineVolume;
            public float backgroundVolume;

            public SavedSettings(float guiVolume, float playerVolume, float machineVolume, float backgroundVolume)
            {
                this.guiVolume = guiVolume;
                this.playerVolume = playerVolume;
                this.machineVolume = machineVolume;
                this.backgroundVolume = backgroundVolume;
            }
        }
    }

}