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

        SavingWrapper savingWrapper;
        private void Awake()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.OnSave.AddListener(UpdateSaveFilesVisualisations);
            UpdateSaveFilesVisualisations();
        }

        private void UpdateSaveFilesVisualisations()
        {
            // TODO: Optimize large amount of save files by only showing the first 10 for example and having a button to show more, or have a limit on how many savefiles there can be

            foreach(Transform oldSaveFile in scrollRectContent)
            {
                if (oldSaveFile.GetComponent<SaveFileVisualisation>() == null) continue;
                Destroy(oldSaveFile.gameObject);
            }

            string[] saveFileDirectories = savingWrapper.GetAllSaveFolders();

            scrollRectContent.sizeDelta = new Vector2(scrollRectContent.rect.width,(250 * saveFileDirectories.Length) +170);
            for (int saveFileIndex = 0; saveFileIndex < saveFileDirectories.Length; saveFileIndex++)
            {
                SaveFileVisualisation saveFile = Instantiate(saveFileVisualisationPrefab, Vector3.zero,Quaternion.identity,scrollRectContent);
                saveFile.transform.localPosition = new Vector3(0, -270 + (-250 * saveFileIndex));

                JsonSavedStatisticData savedData = savingWrapper.LoadStatistics(saveFileDirectories[saveFileIndex]);

                saveFile.SetSaveFileVisualisation(savingWrapper.LoadScreenshot(saveFileDirectories[saveFileIndex]), savedData.timePlayedInSeconds, savedData.machinesBuilt, savedData.conveyorsBuilt,saveFileDirectories[saveFileIndex]);
            }
        }
    }

}