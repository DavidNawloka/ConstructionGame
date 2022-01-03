using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class ShortcutDisplayManager : MonoBehaviour
    {
        [SerializeField] Animator animator;

        ShortcutDisplay[] allShortcutDisplays;

        bool isShownWindow = false;

        private void Awake()
        {
            allShortcutDisplays = FindObjectsOfType<ShortcutDisplay>();
        }

        public void ToggleWindow()
        {
            isShownWindow =  !isShownWindow;

            if (isShownWindow) animator.SetTrigger("show");
            else animator.SetTrigger("hide");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                foreach(ShortcutDisplay shortcutDisplay in allShortcutDisplays)
                {
                    shortcutDisplay.ToggleShortCut();
                }
            }
        }
    }

}