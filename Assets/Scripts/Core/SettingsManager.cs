using Astutos.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using System;
using CON.UI;

namespace CON.Core
{
    public class SettingsManager : MonoBehaviour,ISaveable
    {
        [Header("Audio")]
        [SerializeField] AudioSetting gUIAudio;
        [SerializeField] AudioSetting playerAudio;
        [SerializeField] AudioSetting machineAudio;
        [SerializeField] AudioSetting cutsceneAudio;
        [SerializeField] AudioSetting backgroundMusicAudio;
        [Header("Video")]
        [SerializeField] TMP_Dropdown graphicsQualityDropdown;
        [SerializeField] TMP_Dropdown resolutionDropdown;
        [SerializeField] TMP_Dropdown screenModeDropdown;
        [Header("Controls")]
        [SerializeField] ControlMapping controlMappingPrefab;
        [SerializeField] Transform controlMappingParent;
        [SerializeField] CanvasGroup listenForInputOverlay;
        [SerializeField] float toggleTime = .25f;
        [SerializeField] KeyMapping[] defaultKeyMappings;
        [SerializeField] KeyVisual[] possibleKeys;
        [Header("Desert")]
        [SerializeField] CanvasGroup confirmLeaveOverlay;
        [SerializeField] TextMeshProUGUI lastSaveTimeTMPro;
        [Header("Main Menu")]
        [SerializeField] bool isMainMenu = false;
        [SerializeField] Player.PlayerMouseInteraction.CursorMapping defaultCursorMapping;


        [HideInInspector] public event Action OnInputButtonsChanged;

        SavingSystemJson jsonSaving;
        SavingWrapper savingWrapper;
        CloseButtonManager closeButtonManager;

        Resolution[] resolutions;
        int defaultResolutionIndex;

        Dictionary<string, KeyCode> currentKeyMapping = new Dictionary<string, KeyCode>();

        Dictionary<KeyCode, Sprite> keyMappingVisual = new Dictionary<KeyCode, Sprite>();

        bool shouldListenToInput = true;

        const string settingsFileName = "settings";
        int timeSinceLastSave = 0;

        private void Awake()
        {
            Time.timeScale = 1;
            jsonSaving = FindObjectOfType<SavingSystemJson>();
            closeButtonManager = FindObjectOfType<CloseButtonManager>();

            InitialiseVideoSettings();
            BuildKeyMappingVisualDictionary();
            BuildDefaultKeyMappingDictionary();
            
        }
        private void Start()
        {
            Load();
            if(OnInputButtonsChanged != null)OnInputButtonsChanged();
            Application.quitting += Save;
            if(isMainMenu) Cursor.SetCursor(defaultCursorMapping.sprite, defaultCursorMapping.hotspot, CursorMode.Auto);
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }


        public void ExitGame()
        {
            StartCoroutine(ExitGameDelayed());
        }

        private IEnumerator ExitGameDelayed()
        {
            Save();
            yield return FindObjectOfType<SceneTransitioner>().EndScene();
            Application.Quit();
        }
        public void Save()
        {
             float guiVolume;
             float playerVolume;
             float machineVolume;
             float backgroundVolume;
             float cutsceneVolume;

            gUIAudio.audioMixer.GetFloat("volume", out guiVolume);
            playerAudio.audioMixer.GetFloat("volume", out playerVolume);
            machineAudio.audioMixer.GetFloat("volume", out machineVolume);
            backgroundMusicAudio.audioMixer.GetFloat("volume", out backgroundVolume);
            cutsceneAudio.audioMixer.GetFloat("volume", out cutsceneVolume);


            SerializeableKeyMapping[] serializeableKeyMappings = new SerializeableKeyMapping[currentKeyMapping.Count];

            int index = 0;
            foreach(KeyValuePair<string,KeyCode> keyMapping in currentKeyMapping)
            {
                serializeableKeyMappings[index] = new SerializeableKeyMapping(keyMapping.Key, (int)keyMapping.Value);
                index++;
            }

            SavedSettings savedSettings = new SavedSettings(guiVolume, playerVolume, machineVolume, backgroundVolume, cutsceneVolume, graphicsQualityDropdown.value, resolutionDropdown.value, screenModeDropdown.value, serializeableKeyMappings);
            if (jsonSaving == null) jsonSaving = FindObjectOfType<SavingSystemJson>();
            jsonSaving.Save(settingsFileName, savedSettings);
        }

        public void Load()
        {
            if (jsonSaving == null) jsonSaving = FindObjectOfType<SavingSystemJson>();
            if (jsonSaving == null) return;
            SavedSettings savedSettings = JsonUtility.FromJson<SavedSettings>(jsonSaving.Load(settingsFileName));

            if (savedSettings == null) 
            {
                ResetAudioSettings();
                ResetControlsSettings();
                ResetVideoSettings();
                Save();
                return;
            }

            ChangeGUIVolume(savedSettings.guiVolumeChange);
            gUIAudio.audioSlider.value = savedSettings.guiVolumeChange;

            ChangePlayerVolume(savedSettings.playerVolumeChange);
            playerAudio.audioSlider.value = savedSettings.playerVolumeChange;

            ChangeMachineVolume(savedSettings.machineVolumeChange);
            machineAudio.audioSlider.value = savedSettings.machineVolumeChange;

            ChangeBackgroundVolume(savedSettings.backgroundVolumeChange);
            backgroundMusicAudio.audioSlider.value = savedSettings.backgroundVolumeChange;

            ChangeCutsceneAudio(savedSettings.cutsceneVolumeChange);
            cutsceneAudio.audioSlider.value = savedSettings.cutsceneVolumeChange;

            graphicsQualityDropdown.value = savedSettings.qualityIndex;
            graphicsQualityDropdown.RefreshShownValue();

            resolutionDropdown.value = savedSettings.resolutionIndex;
            resolutionDropdown.RefreshShownValue();

            screenModeDropdown.value = savedSettings.screenModeIndex;
            screenModeDropdown.RefreshShownValue();

            currentKeyMapping.Clear();
            int index = 0;
            foreach(SerializeableKeyMapping savedKeyMapping in savedSettings.serializeableKeyMappings)
            {   
                if(string.IsNullOrWhiteSpace(savedKeyMapping.name) || string.IsNullOrEmpty(savedKeyMapping.name))
                {
                    currentKeyMapping.Add(defaultKeyMappings[index].name, defaultKeyMappings[index].key);
                }
                else currentKeyMapping.Add(savedKeyMapping.name, (KeyCode)savedKeyMapping.keyIndex);

                index++;
            }
            BuildKeyMappingUI();
        }

        // -----------   Desert Leave  -------------

        public void StartExitingToMainMenu()
        {
            closeButtonManager.AddFunction(StayInDesert, "closeOverlay");
            string[] allSaveFolders = savingWrapper.GetAllSaveFolders();
            if(allSaveFolders.Length == 0)
            {
                lastSaveTimeTMPro.text = "You have not saved yet";
            }
            else
            {
                lastSaveTimeTMPro.text = "Your last save was " + savingWrapper.GetTimeString((int)Time.unscaledTime - timeSinceLastSave) + " ago";

            }
            StartCoroutine(SetActiveCanvasGroup(confirmLeaveOverlay, true));
            
        }

        public void FinalExitToMainMenu()
        {
            FindObjectOfType<PauseManager>().LoadScene(0);
        }
        public void StayInDesert()
        {
            closeButtonManager.RemoveFunction("closeOverlay");
            StartCoroutine(SetActiveCanvasGroup(confirmLeaveOverlay, false));
        }

        // -----------   AUDIO -------------

        public void ResetAudioSettings()
        {
            ChangeGUIVolume(0);
            gUIAudio.audioSlider.value = 0;

            ChangePlayerVolume(0);
            playerAudio.audioSlider.value = 0;

            ChangeMachineVolume(0);
            machineAudio.audioSlider.value = 0;

            ChangeBackgroundVolume(0);
            backgroundMusicAudio.audioSlider.value = 0;

            ChangeCutsceneAudio(0);
            cutsceneAudio.audioSlider.value = 0;
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
        public void ChangeCutsceneAudio(float volume)
        {
            ChangeVolume(cutsceneAudio, volume);
        }
        private void ChangeVolume(AudioSetting audioMixer, float volume)
        {
            
            if (Mathf.Approximately(-40f,volume)) volume = -80;
            audioMixer.audioMixer.SetFloat("volume", volume);
        }

        // -----------   Video -------------

        private void InitialiseVideoSettings()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    defaultResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = defaultResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void ResetVideoSettings()
        {
            graphicsQualityDropdown.value = 2;
            graphicsQualityDropdown.RefreshShownValue();

            resolutionDropdown.value = defaultResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            screenModeDropdown.value = 0;
            screenModeDropdown.RefreshShownValue();
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        public void SetScreenMode(int screenModeIndex)
        {
            Screen.fullScreenMode = (FullScreenMode)screenModeIndex;
        }
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        }

        // -----------   CONTROLS  -------------

        public void ResetControlsSettings()
        {
            BuildDefaultKeyMappingDictionary();
            if (OnInputButtonsChanged != null) OnInputButtonsChanged();
            Save();
        }

        public void StartListeningToInput(string buttonName)
        {
            shouldListenToInput = true;
            StartCoroutine(ListenForInput(buttonName));
        }

        public void StopListeningToInput()
        {
            shouldListenToInput = false;
        }

        private IEnumerator ListenForInput(string buttonName)
        {
            if(!isMainMenu) closeButtonManager.SetBlockInput(true);
            yield return StartCoroutine(SetActiveCanvasGroup(listenForInputOverlay, true));
            Array allKeyCodes = Enum.GetValues(typeof(KeyCode));

            while (shouldListenToInput)
            {
                foreach (KeyCode keycode in allKeyCodes)
                {
                    if (Input.GetKeyDown(keycode))
                    {
                        if (UpdateKeyMapping(buttonName, keycode))
                        {
                            shouldListenToInput = false;
                            Save();
                            break;
                        }
                    }
                }
                yield return null;
            }
            yield return StartCoroutine(SetActiveCanvasGroup(listenForInputOverlay, false));

            if (!isMainMenu) closeButtonManager.SetBlockInput(false);
        }

        private IEnumerator SetActiveCanvasGroup(CanvasGroup canvasGroup, bool isActive)
        {
            float timer = 0;

            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;

            if (isActive)
            {
                while (timer < toggleTime)
                {
                    canvasGroup.alpha = timer / toggleTime;
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                canvasGroup.alpha = 1;
            }
            else
            {
                timer = toggleTime;
                while (timer > 0)
                {
                    canvasGroup.alpha = timer / toggleTime;
                    timer -= Time.unscaledDeltaTime;
                    yield return null;
                }
                canvasGroup.alpha = 0;
            }
        }

        public bool UpdateKeyMapping(string name, KeyCode newKey)
        {
            if (!keyMappingVisual.ContainsKey(newKey)) return false;

            if (currentKeyMapping.ContainsValue(newKey) && currentKeyMapping[name] != newKey)
            {
                string sameKeyName = null;
                foreach(KeyValuePair<string,KeyCode> keyValuePair in currentKeyMapping)
                {
                    if (keyValuePair.Value == newKey) sameKeyName = keyValuePair.Key;
                }
                currentKeyMapping[sameKeyName] = currentKeyMapping[name];
                currentKeyMapping[name] = newKey;
            }
            else currentKeyMapping[name] = newKey;


            BuildKeyMappingUI();
            if(OnInputButtonsChanged != null) OnInputButtonsChanged();
            return true;
        }

        private void BuildKeyMappingVisualDictionary()
        {
            foreach(KeyVisual keyVisual in possibleKeys)
            {
                keyMappingVisual.Add(keyVisual.key, keyVisual.sprite);
            }
        }
        private void BuildDefaultKeyMappingDictionary()
        {
            currentKeyMapping.Clear();
            foreach(KeyMapping keyMapping in defaultKeyMappings)
            {
                currentKeyMapping.Add(keyMapping.name, keyMapping.key);
            }
            BuildKeyMappingUI();
        }

        private void BuildKeyMappingUI()
        {
            foreach(Transform child in controlMappingParent)
            {
                if (child.GetComponent<ControlMapping>() == null) continue;
                Destroy(child.gameObject);
            }
            foreach (KeyValuePair<string,KeyCode> keyMapping in currentKeyMapping)
            {
                ControlMapping instance = Instantiate<ControlMapping>(controlMappingPrefab, controlMappingParent);
                instance.InitialiseControlMapping(keyMapping.Key, keyMappingVisual[keyMapping.Value]);
            }
        }
        public KeyCode GetKey(string name)
        {
            return currentKeyMapping[name];
        }

        public Sprite GetKeySprite(string name)
        {
            return keyMappingVisual[currentKeyMapping[name]];
        }

        public object CaptureState()
        {
            Save();
            timeSinceLastSave = (int)Time.unscaledTime;
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
        public class KeyVisual
        {
            public KeyCode key;
            public Sprite sprite;
        }

        [System.Serializable]
        public class KeyMapping
        {
            public string name;
            public KeyCode key;
        }

        [System.Serializable]
        public class SerializeableKeyMapping
        {
            public string name;
            public int keyIndex;

            public SerializeableKeyMapping (string name, int keyIndex)
            {
                this.name = name;
                this.keyIndex = keyIndex;
            }
        }

        [System.Serializable]
        private class SavedSettings
        {
            public float guiVolumeChange;
            public float playerVolumeChange;
            public float machineVolumeChange;
            public float backgroundVolumeChange;
            public float cutsceneVolumeChange;

            public int qualityIndex;
            public int resolutionIndex;
            public int screenModeIndex;

            public SerializeableKeyMapping[] serializeableKeyMappings;

            public SavedSettings(float guiVolumeChange, float playerVolumeChange, float machineVolumeChange, float backgroundVolumeChange, float cutsceneVolumeChange, int qualityIndex, int resolutionIndex, int screenModeIndex, SerializeableKeyMapping[] serializeableKeyMappings)
            {
                this.guiVolumeChange = guiVolumeChange;
                this.playerVolumeChange = playerVolumeChange;
                this.machineVolumeChange = machineVolumeChange;
                this.backgroundVolumeChange = backgroundVolumeChange;
                this.cutsceneVolumeChange = cutsceneVolumeChange;

                this.qualityIndex = qualityIndex;
                this.resolutionIndex = resolutionIndex;
                this.screenModeIndex = screenModeIndex;

                this.serializeableKeyMappings = serializeableKeyMappings;
            }
        }
    }

}