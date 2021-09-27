using Astutos.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CON.UI
{
    public class SaveInitiater : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        SavingWrapper savingWrapper;

        private void Awake()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
            inputField.text = savingWrapper.GetDefaultSaveName();
            EndOfEdit(inputField.text);
        }
        public void EndOfEdit(string saveName)
        {
            if (saveName.Length == 0) return;

            string[] saves = savingWrapper.GetAllSaveFolders();
            int numOfSameName = 0;
            foreach (string save in saves)
            {
                print(save);
                if (save == saveName || (save.Length > saveName.Length && save.Substring(0,saveName.Length) == saveName)) numOfSameName++;
            }

            if (numOfSameName > 0)
            {
                string newSaveName = saveName + numOfSameName.ToString();
                inputField.text = newSaveName;
            }
        }

        public void Save()
        {
            EndOfEdit(inputField.text);
            savingWrapper.StartSave(inputField.text);
            inputField.text = savingWrapper.GetDefaultSaveName();
            EndOfEdit(inputField.text);
        }

    }

}