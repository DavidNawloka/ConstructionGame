
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
        CloseButtonManager escManager;
        SettingsManager settingsManager;

        List<UserInterfaceType> tempClosedUITypes = new List<UserInterfaceType>();

        bool blockInput = false;

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
            if (blockInput) return;
            for (int typeIndex = 0; typeIndex < UITypes.Length; typeIndex++)
            {
                if (UITypes[typeIndex].keyToToggleName == "") continue;

                if (Input.GetKeyDown(UITypes[typeIndex].keyToToggle))
                {
                    ToggleUI(typeIndex);
                }
            }
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
        private void SetActiveUI(int typeIndex, bool isActive)
        {
            UserInterfaceType userInterfaceType = UITypes[typeIndex];
            

            if (!isActive && userInterfaceType.shouldBeShownAlone && tempClosedUITypes.Count != 0)
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
                if (isActive) escManager.AddFunction(() => DeactiveUI(typeIndex), userInterfaceType.GetHashCode().ToString());
                else escManager.RemoveFunction(userInterfaceType.GetHashCode().ToString());
            }
            SetActiveUserInterfaceType(userInterfaceType, isActive);
        }

        private void SetActiveUserInterfaceType(UserInterfaceType UIType, bool isActive)
        {
            if (isActive) UIType.animator.SetTrigger("show");
            else UIType.animator.SetTrigger("hide");

            if (UIType.shouldBeShownAlone && isActive) blockInput = true;
            else blockInput = false;

            UIType.isEnabled = isActive;
            UIType.OnToggle.Invoke();
            if (UIType.audioClip != null) audioSource.PlayOneShot(UIType.audioClip);
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
        }
    }

}