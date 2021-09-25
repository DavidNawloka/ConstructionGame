using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.UI
{

    public class UIManager : MonoBehaviour
    {
        [SerializeField] Transform HUD;
        [SerializeField] Transform PauseMenuUI;

        public UnityEvent<bool> OnPauseStatusChange;

        bool isPaused = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }

        private void TogglePauseMenu()
        {
            isPaused = !isPaused;
            SetPause();
        }

        private void SetPause()
        {
            OnPauseStatusChange.Invoke(isPaused);

            HUD.gameObject.SetActive(!isPaused);
            PauseMenuUI.gameObject.SetActive(isPaused);

            if (isPaused) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }

}