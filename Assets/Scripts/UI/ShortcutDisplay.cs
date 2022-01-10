using CON.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CON.UI 
{
    public class ShortcutDisplay : MonoBehaviour
    {
        [SerializeField] string keyMappingName;
        [SerializeField] Image keySprite;

        CanvasGroup canvasGroup;
        SettingsManager settingsManager;
        bool isShown = false;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            settingsManager = FindObjectOfType<SettingsManager>();
            settingsManager.OnInputButtonsChanged += UpdateKeySprite;
        }
        private void Start()
        {
            UpdateKeySprite();
        }

        private void UpdateKeySprite()
        {
            keySprite.sprite = settingsManager.GetKeySprite(keyMappingName);
        }

        public void ToggleShortCut()
        {
            isShown = !isShown;
            SetActiveShortcut();
        }

        private void SetActiveShortcut()
        {
            if (isShown) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;

            canvasGroup.interactable = isShown;
            canvasGroup.blocksRaycasts = isShown;
        }
    }
}
