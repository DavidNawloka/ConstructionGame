using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.UI
{
    public class UserInterfaceManager : MonoBehaviour
    {
        [SerializeField] UserInterfaceType[] UITypes;

        AudioSource audioSource;
        EscManager escManager;

        List<UserInterfaceType> tempClosedUITypes = new List<UserInterfaceType>();

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            escManager = FindObjectOfType<EscManager>();
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
            userInterfaceType.OnToggle.Invoke();

            if(!isActive && userInterfaceType.shouldBeShownAlone && tempClosedUITypes.Count != 0)
            {
                foreach (UserInterfaceType UIType in tempClosedUITypes)
                {
                    SetActiveUserInterfaceType(UIType, true);
                }
                tempClosedUITypes.Clear();
            }

            if (isActive && userInterfaceType.shouldBeShownAlone)
            {
                foreach (UserInterfaceType UIType in UITypes)
                {
                    if (!UIType.isEnabled) continue;

                    SetActiveUserInterfaceType(UIType, false);
                    tempClosedUITypes.Add(UIType);
                }
                
            }

            if (userInterfaceType.closedByEsc)
            {
                if (isActive) escManager.AddEscFunction(() => DeactiveUI(typeIndex), userInterfaceType.GetHashCode().ToString());
                else escManager.RemoveESCFunction(userInterfaceType.GetHashCode().ToString());
            }

            SetActiveCanvasGroup(userInterfaceType.canvasGroup, isActive);
            userInterfaceType.isEnabled = isActive;
        }

        private void SetActiveUserInterfaceType(UserInterfaceType UIType, bool isActive)
        {
            SetActiveCanvasGroup(UIType.canvasGroup, isActive);
            UIType.isEnabled = isActive;
            UIType.OnToggle.Invoke();
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
            public bool isEnabled;
            public bool closedByEsc;
            public AudioClip audioClip;
            public UnityEvent OnToggle;
        }
    }

}