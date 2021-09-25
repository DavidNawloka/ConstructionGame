using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astutos.Saving
{
    public class SavingWrapper : MonoBehaviour
    {
        string saveFileName = "testSaveFile";
        SavingSystemEncrypted savingSystemEncrypted;
        SavingSystemJson savingSystemJson;

        int timePlayedInSeconds;

        private void Awake()
        {
            savingSystemEncrypted = GetComponent<SavingSystemEncrypted>();
            savingSystemJson = GetComponent<SavingSystemJson>();
            Load();
        }
        
        public void Save() // Button OnClick Event
        {
            savingSystemEncrypted.Save(saveFileName);
            savingSystemJson.Save(saveFileName, GetGameStatistics());
        }

        public void Load() // Button OnClick Event
        {
            savingSystemEncrypted.Load(saveFileName);
            JsonSaveData savedData = savingSystemJson.Load(saveFileName);

            if (savedData == null) return;
            timePlayedInSeconds = savedData.seconds;

            print(string.Format("Time played: {0} | Machines Built: {1} | Coneyors Built: {2}",savedData.seconds,savedData.machinesBuilt,savedData.conveyorsBuilt));
        }

        private JsonSaveData GetGameStatistics()
        {
            int timePlayedAll = ((int)Time.time) + timePlayedInSeconds;
            int machinesBuilt = FindObjectsOfType<Machine>().Length;
            int conveyorsBuilt = FindObjectsOfType<Conveyor>().Length + FindObjectsOfType<SeperatorConveyor>().Length;
            return new JsonSaveData(timePlayedAll, machinesBuilt, conveyorsBuilt);
        }

    }
    [System.Serializable]
    public class JsonSaveData
    {
        public int seconds;
        public int machinesBuilt;
        public int conveyorsBuilt;

        public JsonSaveData(int seconds, int machinesBuilt, int conveyorsBuilt)
        {
            this.seconds = seconds;
            this.machinesBuilt = machinesBuilt;
            this.conveyorsBuilt = conveyorsBuilt;
        }
    }
}
