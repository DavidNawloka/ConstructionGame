using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Core
{
    public class EscManager : MonoBehaviour
    {
        [SerializeField] UnityEvent OnEscResponsesStackEmpty;

        Stack<EscResponse> responsesForEscape = new Stack<EscResponse>();

        public void AddEscFunction(Action action, string hash)
        {
            responsesForEscape.Push(new EscResponse(action,hash));
        }
        public void RemoveESCFunction(string hash)
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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapePressed();
            }
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.F3))
            {
                foreach (EscResponse savedResponses in responsesForEscape)
                {
                    print(savedResponses.hash);
                }
            }
        }

        private void EscapePressed()
        {
            if (responsesForEscape.Count == 0)
            {
                OnEscResponsesStackEmpty.Invoke();
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