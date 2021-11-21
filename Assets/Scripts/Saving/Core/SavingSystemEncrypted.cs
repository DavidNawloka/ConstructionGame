using System.Collections;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using CON.Core;

namespace Astutos.Saving
{
    public class SavingSystemEncrypted : MonoBehaviour
    {
        const string buildIndexKey = "lastSceneBuildIndex";
        const string filenameExtension = "sav";

        public IEnumerator LoadLastScene(string saveFile)
        {
            yield return FindObjectOfType<SceneTransitioner>().EndScene();
            float time = Time.time;
            Dictionary<string, object> state = LoadFile(saveFile);

            int lastSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey(buildIndexKey))
            {
                lastSceneIndex = (int)state[buildIndexKey];
            }
            yield return SceneManager.LoadSceneAsync(lastSceneIndex); // All Awake Methods of the new scene have been called after continueing this code block
            time = Time.time - time;
            print(time);
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }


        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }
        public void Delete(string saveFile)
        {
            DeleteFile(saveFile);
        }


        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                var rng = new RNGCryptoServiceProvider();
                var newKey = new byte[32];
                var newIV = new byte[16];
                rng.GetNonZeroBytes(newKey);
                rng.GetNonZeroBytes(newIV);
                stream.Write(newKey, 0, newKey.Length);
                stream.Write(newIV, 0, newIV.Length);

                using (CryptoStream cryptStream = new CryptoStream(stream, aes.CreateEncryptor(newKey, newIV), CryptoStreamMode.Write))
                {

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(cryptStream, state);
                }
            }
        }
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {

                var newKey = new byte[32];
                var newIV = new byte[16];
                stream.Read(newKey, 0, newKey.Length);
                stream.Read(newIV, 0, newIV.Length);
                using (CryptoStream cryptStream = new CryptoStream(stream, aes.CreateDecryptor(newKey, newIV), CryptoStreamMode.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    return (Dictionary<string, object>)formatter.Deserialize(cryptStream);
                }
            }
        }

        public void DeleteFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path)) return;

            File.Delete(path);
        }
        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            state[buildIndexKey] = SceneManager.GetActiveScene().buildIndex;
        }
        private void RestoreState(Dictionary<string, object> state)
        {
            if (state.Count == 0) return;
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (!state.ContainsKey(id)) continue;

                saveable.RestoreState(state[id]);
            }
        }
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ("." + filenameExtension));
        }
    }
}