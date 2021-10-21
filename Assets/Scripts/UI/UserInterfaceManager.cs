using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class UserInterfaceManager : MonoBehaviour
    {
        [SerializeField] UserInterfaceType[] UITypes;

        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void ActivateUI(int typeIndex)
        {
            SetActiveUI(typeIndex, true);
        }
        public void DeactiveUI(int typeIndex)
        {
            SetActiveUI(typeIndex, false);
        }
        public void ToggleUI(int typeIndex)
        {
            SetActiveUI(typeIndex,!UITypes[typeIndex].canvasGroup.interactable);
        }
        private void SetActiveUI(int typeIndex, bool isActive)
        {
            UserInterfaceType userInterfaceType = UITypes[typeIndex];
            if(userInterfaceType.audioClip != null) audioSource.PlayOneShot(userInterfaceType.audioClip);
            if (isActive && userInterfaceType.shouldBeShownAlone)
            {
                foreach (UserInterfaceType UIType in UITypes)
                {
                    SetActiveCanvasGroup(UIType.canvasGroup, false); // Need way of reenabeling these disabled UI TODO: FIX UI System
                }

            }
            SetActiveCanvasGroup(userInterfaceType.canvasGroup, isActive);
        }

        private void SetActiveCanvasGroup(CanvasGroup canvasGroup, bool isActive)
        {
            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;

            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }
        [System.Serializable]
        public class UserInterfaceType
        {
            public CanvasGroup canvasGroup;
            public bool shouldBeShownAlone;
            public AudioClip audioClip;
        }
    }

}