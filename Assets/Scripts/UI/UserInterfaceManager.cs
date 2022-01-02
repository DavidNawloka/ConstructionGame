
using CON.Core;
using System;
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
        CloseButtonManager escManager;
        SettingsManager settingsManager;

        List<UserInterfaceType> tempClosedUITypes = new List<UserInterfaceType>();
        int onlyInputUITypeIndex = -1;

        bool inputDisabled = false;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            escManager = FindObjectOfType<CloseButtonManager>();
            settingsManager = FindObjectOfType<SettingsManager>();

            settingsManager.OnInputButtonsChanged += UpdateInputMapping;
            
        }
        private void Start()
        {
            foreach(UserInterfaceType UIType in UITypes)
            {
                if(UIType.isEnabled) UIType.animator.SetTrigger("show");
            }
        }

        private void UpdateInputMapping()
        {
            foreach (UserInterfaceType UIType in UITypes)
            {
                if (UIType.keyToToggleName != "") UIType.keyToToggle = settingsManager.GetKey(UIType.keyToToggleName);
            }
        }

        private void Update()
        {
            if (inputDisabled) return;
            HandleInput();
        }

        private void HandleInput()
        {
            for (int typeIndex = 0; typeIndex < UITypes.Length; typeIndex++)
            {
                if (UITypes[typeIndex].keyToToggleName == "" || (onlyInputUITypeIndex != -1 && onlyInputUITypeIndex != typeIndex)) continue;

                if (Input.GetKeyDown(UITypes[typeIndex].keyToToggle))
                {
                    ToggleUI(typeIndex);
                }
            }
        }
        public bool IsUserInterfaceTypeActive(int typeIndex)
        {
            return UITypes[typeIndex].isEnabled;
        }
        public void OnInputDeactivationChange(bool isDisabled) // Input Allowance Class Event
        {
            inputDisabled = isDisabled;
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
            SetActiveUI(typeIndex, !UITypes[typeIndex].isEnabled);
        }
        public void SetActiveUIStripped(int userInteraceIndex, bool isActive)
        {
            SetActiveUserInterfaceType(UITypes[userInteraceIndex], isActive, false);
        }
        private void SetActiveUI(int typeIndex, bool isActive)
        {
            UserInterfaceType userInterfaceType = UITypes[typeIndex];

            SetActiveUserInterfaceType(userInterfaceType, isActive, true);
            HandleClosedByESC(typeIndex, isActive);

            if (userInterfaceType.shouldBeShownAlone) HandleShownAloneUIType(typeIndex, isActive, userInterfaceType);

        }

        private void HandleClosedByESC(int typeIndex, bool isActive)
        {
            UserInterfaceType userInterfaceType = UITypes[typeIndex];
            if (userInterfaceType.closedByEsc)
            {
                if (isActive) escManager.AddFunction(() => DeactiveUI(typeIndex), userInterfaceType.GetHashCode().ToString());
                else escManager.RemoveFunction(userInterfaceType.GetHashCode().ToString());
            }
        }

        private void HandleShownAloneUIType(int typeIndex, bool isActive, UserInterfaceType userInterfaceType)
        {
            if (isActive)
            {
                onlyInputUITypeIndex = typeIndex;
                foreach (UserInterfaceType UIType in UITypes)
                {
                    if (!UIType.isEnabled || UIType == userInterfaceType) continue;
                    
                    SetActiveUserInterfaceType(UIType, false, false);
                    
                    HandleClosedByESC(GetUITypeIndex(UIType), false);
                    userInterfaceType.tempClosedUI.Add(GetUITypeIndex(UIType));
                }
            }
            else
            {
                foreach (int UITypeIndex in userInterfaceType.tempClosedUI)
                {
                    SetActiveUserInterfaceType(UITypes[UITypeIndex], true, false);
                    HandleClosedByESC(UITypeIndex, true);
                    if (UITypes[UITypeIndex].shouldBeShownAlone) onlyInputUITypeIndex = UITypeIndex;
                }
                userInterfaceType.tempClosedUI.Clear();
                if(onlyInputUITypeIndex == typeIndex) onlyInputUITypeIndex = -1;
            }
        }
        private int GetUITypeIndex(UserInterfaceType userInterfaceType)
        {
            return Array.IndexOf(UITypes, userInterfaceType);
        }

        private void SetActiveUserInterfaceType(UserInterfaceType UIType, bool isActive, bool shouldPlaySound)
        {
            //print(isActive + ":" + GetUITypeIndex(UIType));
            if (isActive) UIType.animator.SetTrigger("show");
            else UIType.animator.SetTrigger("hide");

            UIType.isEnabled = isActive;
            UIType.OnToggle.Invoke();
            if (shouldPlaySound && UIType.audioClip != null) audioSource.PlayOneShot(UIType.audioClip);
        }
        [System.Serializable]
        public class UserInterfaceType
        {
            public Animator animator;
            public bool shouldBeShownAlone;
            public bool isEnabled;
            public bool closedByEsc;
            public string keyToToggleName;
            public AudioClip audioClip;
            public UnityEvent OnToggle;
            [HideInInspector] public KeyCode keyToToggle;
            public List<int> tempClosedUI = new List<int>();
        }
    }

}