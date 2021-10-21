using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.UI
{

    public class PauseManager : MonoBehaviour
    {
        [SerializeField] Transform BuildModeUI;

        public UnityEvent<bool> OnPauseStatusChange;

        bool isPaused = false;

        UserInterfaceManager userInterfaceManager;

        private void Awake()
        {
            userInterfaceManager = FindObjectOfType<UserInterfaceManager>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }

        private void TogglePauseMenu()
        {
            userInterfaceManager.ToggleUI(3);
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