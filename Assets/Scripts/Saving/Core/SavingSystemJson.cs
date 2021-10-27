using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Astutos.Saving
{
    public class SavingSystemJson : MonoBehaviour
    {
        public void Save(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            string jsonState = JsonUtility.ToJson(state);
            File.WriteAllText(path, jsonState);

        }
        public string Load(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path)) return null;

            string jsonState = File.ReadAllText(path);
            return jsonState;
        }
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + (".json"));
        }
    }

}