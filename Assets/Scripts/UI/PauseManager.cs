using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.UI
{

    public class PauseManager : MonoBehaviour
    {
        [SerializeField] Transform HUD;
        [SerializeField] Transform BuildModeUI;

        public UnityEvent<bool> OnPauseStatusChange;

        bool isPaused = false;
        CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            SetCanvasGroup(false);
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
            isPaused = !isPaused;
            SetPause();
        }

        private void SetPause()
        {
            OnPauseStatusChange.Invoke(isPaused);

            HUD.gameObject.SetActive(!isPaused);
            BuildModeUI.gameObject.SetActive(!isPaused);
            SetCanvasGroup(isPaused);

            if (isPaused) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
        private void SetCanvasGroup(bool isActive)
        {
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;

            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;
        }
    }

}