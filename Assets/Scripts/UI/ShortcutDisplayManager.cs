using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CON.UI
{
    public class ShortcutDisplayManager : MonoBehaviour
    {
        [SerializeField] string toggleShortcutDisplayButton;
        [SerializeField] Animator animator;
        [SerializeField] Image keyImage;
 
        ShortcutDisplay[] allShortcutDisplays;
        KeyCode toggleShortcutDisplay;

        bool isShownWindow = false;

        SettingsManager settingsManager;


        private void Awake()
        {
            settingsManager = FindObjectOfType<SettingsManager>();
            allShortcutDisplays = FindObjectsOfType<ShortcutDisplay>();
            settingsManager.OnInputButtonsChanged += UpdateButton;
        }

        private void UpdateButton()
        {
            toggleShortcutDisplay = settingsManager.GetKey(toggleShortcutDisplayButton);
            keyImage.sprite = settingsManager.GetKeySprite(toggleShortcutDisplayButton);
        }

        public void ToggleWindow()
        {
            isShownWindow =  !isShownWindow;

            if (isShownWindow) animator.SetTrigger("show");
            else animator.SetTrigger("hide");
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleShortcutDisplay))
            {
                foreach(ShortcutDisplay shortcutDisplay in allShortcutDisplays)
                {
                    shortcutDisplay.ToggleShortCut();
                }
            }
        }
    }

}