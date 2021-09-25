using CON.Core;
using CON.Machines;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Astutos.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        public UnityEvent OnSave;

        const string MAIN_SAVE_FILE_NAME = "mainSaveFile";
        const string STATISTIC_SAVE_FILE_NAME = "statisticsSaveFile";
        const string SCREENSHOT_SAVE_FILE_NAME = "screenshot";

        string defaultSaveFolderName = "saveFile";

        SavingSystemEncrypted savingSystemEncrypted;
        SavingSystemJson savingSystemJson;
        ScreenCaptureSaving screenCapture;

        int timePlayedInSeconds;
        int saveFileNum = 0;

        private void Awake()
        {
            savingSystemEncrypted = GetComponent<SavingSystemEncrypted>();
            savingSystemJson = GetComponent<SavingSystemJson>();
            screenCapture = FindObjectOfType<ScreenCaptureSaving>();

            saveFileNum = Directory.GetDirectories(Application.persistentDataPath).Length - 1;
            Load(defaultSaveFolderName + (saveFileNum).ToString());
            saveFileNum++;
        }
        
        public void StartSave()// Button OnClick Event
        {
            StartCoroutine(Save());
        }

        public IEnumerator Save() 
        {
            string saveFolderName = defaultSaveFolderName + saveFileNum++;
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, saveFolderName));

            savingSystemEncrypted.Save(GetPathWithFolder(saveFolderName,MAIN_SAVE_FILE_NAME));
            savingSystemJson.Save(GetPathWithFolder(saveFolderName,STATISTIC_SAVE_FILE_NAME), GetGameStatistics());
            yield return screenCapture.CaptureAndSaveScreenshot(GetPathWithFolder(saveFolderName,SCREENSHOT_SAVE_FILE_NAME));

            OnSave.Invoke();
        }

        public void Load(string saveFolderName) // Button OnClick Event
        {
            savingSystemEncrypted.Load(GetPathWithFolder(saveFolderName, MAIN_SAVE_FILE_NAME));
            JsonSavedStatisticData savedData = savingSystemJson.Load(GetPathWithFolder(saveFolderName, STATISTIC_SAVE_FILE_NAME));

            if (savedData == null) return;
            timePlayedInSeconds = savedData.timePlayedInSeconds;
        }

        public JsonSavedStatisticData LoadStatistics(string folderPath)
        {
            JsonSavedStatisticData savedData = savingSystemJson.Load(Path.Combine(folderPath, STATISTIC_SAVE_FILE_NAME));

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
            int conveyorsBuilt = FindObjectsOfType<Conveyor>().Length + FindObjectsOfType<SeperatorConveyor>().Length;
            return new JsonSavedStatisticData(timePlayedAll, machinesBuilt, conveyorsBuilt, saveFileNum + 1);
        }

        private string GetPathWithFolder(string folderName, string saveFileName)
        {
            return Path.Combine(folderName, saveFileName);
        }

    }
    [System.Serializable]
    public class JsonSavedStatisticData
    {
        public int timePlayedInSeconds;
        public int machinesBuilt;
        public int conveyorsBuilt;
        public int saveFileNum;

        public JsonSavedStatisticData(int timePlayedInSeconds, int machinesBuilt, int conveyorsBuilt, int saveFileNum)
        {
            this.timePlayedInSeconds = timePlayedInSeconds;
            this.machinesBuilt = machinesBuilt;
            this.conveyorsBuilt = conveyorsBuilt;
            this.saveFileNum = saveFileNum;
        }
    }
}
