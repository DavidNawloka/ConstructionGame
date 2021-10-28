using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Core
{
    public class CloseButtonManager : MonoBehaviour
    {
        [SerializeField] string closeWindowButtonName;
        [SerializeField] UnityEvent OnResponsesStackEmpty;

        Stack<EscResponse> responsesForEscape = new Stack<EscResponse>();

        SettingsManager settingsManager;

        KeyCode closeWindowButton;

        bool blockInput = false;

        private void Awake()
        {
            settingsManager = FindObjectOfType<SettingsManager>();
            settingsManager.OnInputButtonsChanged += UpdateButtonMapping;
        }

        private void UpdateButtonMapping()
        {
            closeWindowButton = settingsManager.GetKey(closeWindowButtonName);
        }

        public void SetBlockInput(bool shouldBlock)
        {
            blockInput = shouldBlock;
        }

        public void AddFunction(Action action, string hash)
        {
            responsesForEscape.Push(new EscResponse(action,hash));
        }
        public void RemoveFunction(string hash)
        {
            Stack<EscResponse> newResponsesForEscape = new Stack<EscResponse>();

            EscResponse[] escResponsesArray = responsesForEscape.ToArray();

            for (int index = escResponsesArray.Length-1; index >= 0 ; index--)
            {
                if (escResponsesArray[index].hash == hash) continue;

                newResponsesForEscape.Push(new EscResponse(escResponsesArray[index]));
            }
            responsesForEscape = newResponsesForEscape;
        }
        private void Update()
        {
            if (blockInput) return;

            if (Input.GetKeyDown(closeWindowButton))
            {
                ButtonPressed();
            }
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F3))
            {
                foreach (EscResponse savedResponses in responsesForEscape)
                {
                    print(savedResponses.hash);
                }
            }
        }

        private void ButtonPressed()
        {
            if (responsesForEscape.Count == 0)
            {
                OnResponsesStackEmpty.Invoke();
                return;
            }
            responsesForEscape.Pop().action();
        }

        class EscResponse
        {
            public Action action;
            public string hash;

            public EscResponse(Action action, string hash)
            {
                this.action = action;
                this.hash = hash;
            }
            public EscResponse(EscResponse escFunction)
            {
                action = escFunction.action;
                hash = escFunction.hash;
            }
        }
    }

}