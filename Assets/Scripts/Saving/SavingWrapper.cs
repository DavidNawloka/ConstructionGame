using CON.Core;
using CON.Machines;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Astutos.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        public UnityEvent OnSaveFileChange;

        const string MAIN_SAVE_FILE_NAME = "mainSaveFile";
        const string STATISTIC_SAVE_FILE_NAME = "statisticsSaveFile";
        const string SCREENSHOT_SAVE_FILE_NAME = "screenshot";

        string defaultSaveFolderName = "saveFile";

        SavingSystemEncrypted savingSystemEncrypted;
        SavingSystemJson savingSystemJson;
        ScreenCaptureSaving screenCapture;
        SceneTransitioner sceneTransitioner;

        int timePlayedInSeconds;
        int saveFileNum = 0;

        private void Awake()
        {
            savingSystemEncrypted = GetComponent<SavingSystemEncrypted>();
            savingSystemJson = GetComponent<SavingSystemJson>();
            screenCapture = FindObjectOfType<ScreenCaptureSaving>();
            sceneTransitioner = FindObjectOfType<SceneTransitioner>();

            saveFileNum = Directory.GetDirectories(Application.persistentDataPath).Length;

        }
        
        public void StartSave(string saveName)// Button OnClick Event
        {
            StartCoroutine(Save(saveName));
        }

        public IEnumerator Save(string saveName) 
        {
            saveFileNum++;
            string saveFolderName;
            if (saveName.Length == 0)
            {
                saveFolderName = defaultSaveFolderName + saveFileNum.ToString();
            }
            else
            {
                saveFolderName = saveName;
            }
            
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, saveFolderName));

            savingSystemEncrypted.Save(GetPathWithFolder(saveFolderName,MAIN_SAVE_FILE_NAME));
            savingSystemJson.Save(GetPathWithFolder(saveFolderName,STATISTIC_SAVE_FILE_NAME), GetGameStatistics());
            yield return screenCapture.CaptureAndSaveScreenshot(GetPathWithFolder(saveFolderName,SCREENSHOT_SAVE_FILE_NAME));

            OnSaveFileChange.Invoke();
        }

        public void Load(string saveFolderName)
        {
            StartCoroutine(savingSystemEncrypted.LoadLastScene(GetPathWithFolder(saveFolderName, MAIN_SAVE_FILE_NAME)));

            JsonSavedStatisticData savedData = JsonUtility.FromJson<JsonSavedStatisticData>(savingSystemJson.Load(GetPathWithFolder(saveFolderName, STATISTIC_SAVE_FILE_NAME)));

            if (savedData == null) return;
            timePlayedInSeconds = savedData.timePlayedInSeconds;
        }

        public void LoadLastSave()
        {
            string[] allSaveFolders = GetAllSaveFolders();
            Load(allSaveFolders[0]);
        }

        public void Delete(string saveFolderName)
        {
            Directory.Delete(Path.Combine(Application.persistentDataPath, saveFolderName),true);
            OnSaveFileChange.Invoke();
        }

        public string GetDefaultSaveName()
        {
            return defaultSaveFolderName;
        }
        public string[] GetAllSaveFolders()
        {
            string[] savesWithPath = Directory.GetDirectories(Application.persistentDataPath);
            FileInfo[] savesFileInfo = new FileInfo[savesWithPath.Length];

            for (int saveIndex = 0; saveIndex < savesWithPath.Length; saveIndex++)
            {
                savesFileInfo[saveIndex] = new FileInfo(savesWithPath[saveIndex]);
            }
            Array.Sort(savesFileInfo, delegate (FileInfo fileInfo1, FileInfo fileInfo2) { return fileInfo2.CreationTime.CompareTo(fileInfo1.CreationTime); });
            string[] saves = new string[savesWithPath.Length];

            for (int saveIndex = 0; saveIndex < saves.Length; saveIndex++)
            {
                saves[saveIndex] = savesFileInfo[saveIndex].Name;
            }
            return saves;
        }
        public JsonSavedStatisticData LoadStatistics(string saveFolderName)
        {
            JsonSavedStatisticData savedData = JsonUtility.FromJson<JsonSavedStatisticData>(savingSystemJson.Load(Path.Combine(Application.persistentDataPath,saveFolderName, STATISTIC_SAVE_FILE_NAME)));

            if (savedData == null) return null;
            return savedData;
        }
        public Texture2D LoadScreenshot(string folderPath)
        {
            return screenCapture.LoadScreenshot(GetPathWithFolder(folderPath, SCREENSHOT_SAVE_FILE_NAME));
        }

        private JsonSavedStatisticData GetGameStatistics()
        {
            int timePlayedAll = ((int)Time.time) + timePlayedInSeconds;
            int machinesBuilt = FindObjectsOfType<Machine>().Length;
            int conveyorsBuilt = FindObjectsOfType<Conveyor>().Length + FindObjectsOfType<SplitterConveyor>().Length;
            return new JsonSavedStatisticData(timePlayedAll, machinesBuilt, conveyorsBuilt);
        }

        private string GetPathWithFolder(string folderName, string saveFileName)
        {
            return Path.Combine(folderName, saveFileName);
        }
        public string GetTimeString(int timePlayed)
        {
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            if (timePlayed < 60) seconds = timePlayed;
            else
            {
                seconds = timePlayed % 60;
                int division1 = Mathf.FloorToInt(timePlayed / 60);

                if (division1 < 60) minutes = division1;
                else
                {
                    minutes = division1 % 60;
                    int division2 = Mathf.FloorToInt(division1 / 60);
                    hours = division2;
                }
            }

            return string.Format("{0}h {1}m {2}s", hours, minutes, seconds);
        }

    }
    [System.Serializable]
    public class JsonSavedStatisticData
    {
        public int timePlayedInSeconds;
        public int machinesBuilt;
        public int conveyorsBuilt;

        public JsonSavedStatisticData(int timePlayedInSeconds, int machinesBuilt, int conveyorsBuilt)
        {
            this.timePlayedInSeconds = timePlayedInSeconds;
            this.machinesBuilt = machinesBuilt;
            this.conveyorsBuilt = conveyorsBuilt;
        }
    }
}
