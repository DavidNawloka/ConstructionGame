using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Astutos.Saving;

namespace CON.UI
{
    public class SaveFileVisualisation : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI saveFileName;
        [SerializeField] TextMeshProUGUI timePlayedAmount;
        [SerializeField] TextMeshProUGUI machinesBuiltAmount;
        [SerializeField] TextMeshProUGUI conveyorsBuiltAmount;
        [SerializeField] RawImage screenshotImage;

        string saveFolderName;
        SavingWrapper savingWrapper;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        public void DeleteSaveFile()
        {
            savingWrapper.Delete(saveFolderName);
        }

        public void SetSaveFileVisualisation(Texture2D screenshot, int timePlayed, int machinesBuilt, int conveyorsBuilt,string mainSaveFileName)
        {
            screenshotImage.texture = screenshot;
            timePlayedAmount.text = GetTimeString(timePlayed);
            machinesBuiltAmount.text = machinesBuilt.ToString();
            conveyorsBuiltAmount.text = conveyorsBuilt.ToString();
            saveFileName.text = mainSaveFileName;
            saveFolderName = mainSaveFileName;
        }

        private string GetTimeString(int timePlayed)
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

        private void OnClick()
        {
            savingWrapper.Load(saveFolderName);
        }
    }
}
