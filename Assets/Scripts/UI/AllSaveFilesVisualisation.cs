using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astutos.Saving;

namespace CON.UI
{
    public class AllSaveFilesVisualisation : MonoBehaviour
    {
        [SerializeField] SaveFileVisualisation saveFileVisualisationPrefab;
        [SerializeField] RectTransform scrollRectContent;
        [SerializeField] int defaultShownSaves = 5;
        [SerializeField] int shownSavesChangePerClick = 5;

        SavingWrapper savingWrapper;

        int shownSaves;

        private void Awake()
        {
            shownSaves = defaultShownSaves;
        }
        private void Start()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
            UpdateSaveFilesVisualisation();
            savingWrapper.OnSaveFileChange.AddListener(UpdateSaveFilesVisualisation);
        }
        private void OnEnable()
        {
            shownSaves = defaultShownSaves;
        }


        public void IncreaseShownSaves() // Button OnClick event function
        {
            shownSaves += shownSavesChangePerClick;
            UpdateSaveFilesVisualisation();
        }

        public void DecreaseShownSaves() // Button OnClick event function
        {
            shownSaves -= shownSavesChangePerClick;
            UpdateSaveFilesVisualisation();
        }

        private void UpdateSaveFilesVisualisation()
        {
            // TODO: Optimize large amount of save files

            foreach(Transform oldSaveFile in scrollRectContent)
            {
                if (oldSaveFile.GetComponent<SaveFileVisualisation>() == null) continue;
                Destroy(oldSaveFile.gameObject);
            }

            string[] saveFileDirectories = savingWrapper.GetAllSaveFolders();

            shownSaves = Mathf.Clamp(shownSaves, Mathf.Min(defaultShownSaves,saveFileDirectories.Length), saveFileDirectories.Length);

            scrollRectContent.sizeDelta = new Vector2(scrollRectContent.rect.width,((saveFileVisualisationPrefab.GetComponent<RectTransform>().rect.height + 50) * shownSaves) +300);
            for (int saveFileIndex = 0; saveFileIndex < shownSaves; saveFileIndex++)
            {
                SaveFileVisualisation saveFile = Instantiate(saveFileVisualisationPrefab, Vector3.zero,Quaternion.identity,scrollRectContent);

                JsonSavedStatisticData savedData = savingWrapper.LoadStatistics(saveFileDirectories[saveFileIndex]);

                saveFile.SetSaveFileVisualisation(savingWrapper.LoadScreenshot(saveFileDirectories[saveFileIndex]), savedData.timePlayedInSeconds, savedData.machinesBuilt, savedData.conveyorsBuilt,saveFileDirectories[saveFileIndex]);
            }
        }
    }

}