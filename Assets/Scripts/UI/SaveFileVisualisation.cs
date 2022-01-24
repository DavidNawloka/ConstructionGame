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

        public void SetSaveFileVisualisation(Texture2D screenshot, string timePlayed, int machinesBuilt, int conveyorsBuilt,string mainSaveFileName)
        {
            screenshotImage.texture = screenshot;
            timePlayedAmount.text = timePlayed;
            machinesBuiltAmount.text = machinesBuilt.ToString();
            conveyorsBuiltAmount.text = conveyorsBuilt.ToString();
            saveFileName.text = mainSaveFileName;
            saveFolderName = mainSaveFileName;
        }

        

        private void OnClick()
        {
            savingWrapper.Load(saveFolderName);
        }
    }
}
