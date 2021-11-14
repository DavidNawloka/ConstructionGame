using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CON.UI
{

    public class PauseManager : MonoBehaviour
    {
        public UnityEvent<bool> OnPauseStatusChange;

        bool isPaused = false;

        UserInterfaceManager userInterfaceManager;

        private void Awake()
        {
            userInterfaceManager = FindObjectOfType<UserInterfaceManager>();
            Time.timeScale = 1;
        }

        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void TogglePauseMenu()
        {
            userInterfaceManager.ToggleUI(4);
            isPaused = !isPaused;
            SetPause();
        }

        private void SetPause()
        {
            OnPauseStatusChange.Invoke(isPaused);

            if (isPaused) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }

}