using Astutos.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CON.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI continueText;
        [SerializeField] Button continueButton;

        SavingWrapper savingWrapper;


        private void Awake()
        {
            Time.timeScale = 1;
        }
        private void Start()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.OnSaveFileChange.AddListener(UpdateMainButton);
            UpdateMainButton();
        }

        private void UpdateMainButton()
        {
            if (savingWrapper.GetAllSaveFolders().Length == 0)
            {
                continueText.text = "New Game";
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(LoadMainScene);
            }
            else
            {
                continueText.text = "Continue";
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(ContinueLastSave);
            }
        }

        public void ContinueLastSave()
        {
            savingWrapper.LoadLastSave();
        }
        public void LoadMainScene()
        {
            SceneManager.LoadScene(1);
        }
    }

}