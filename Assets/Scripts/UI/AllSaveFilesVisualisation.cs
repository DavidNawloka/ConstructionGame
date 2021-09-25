using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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
            foreach(Transform oldSaveFile in scrollRectContent)
            {
                Destroy(oldSaveFile.gameObject);
            }

            string[] saveFileDirectories = Directory.GetDirectories(Application.persistentDataPath);

            scrollRectContent.sizeDelta = new Vector2(scrollRectContent.rect.width,(250 * saveFileDirectories.Length) +50);
            for (int saveFileIndex = 0; saveFileIndex < saveFileDirectories.Length; saveFileIndex++)
            {
                SaveFileVisualisation saveFile = Instantiate(saveFileVisualisationPrefab, Vector3.zero,Quaternion.identity,scrollRectContent);
                saveFile.transform.localPosition = new Vector3(0, -150 + (-250 * saveFileIndex));

                JsonSavedStatisticData savedData = savingWrapper.LoadStatistics(saveFileDirectories[saveFileIndex]);

                saveFile.SetSaveFileVisualisation(savingWrapper.LoadScreenshot(Path.GetFileName(saveFileDirectories[saveFileIndex])), savedData.timePlayedInSeconds, savedData.machinesBuilt, savedData.conveyorsBuilt,Path.GetFileName(saveFileDirectories[saveFileIndex]));
            }
        }
    }

}