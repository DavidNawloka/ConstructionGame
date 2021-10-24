using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    public class ESCManager : MonoBehaviour
    {
        // TODO: Functions can subscribe to AddEscFunction, if escape pressed dequeue and run function, if function gets run before dequeued remove from queue (e.g. exit buildmode (with b before esc))
        // Signups are: MoveableWindow, BuildMode, Demolish Mode, Placement Mode, Pause Menu (if queue empty)
        Queue<Action> functionsForEscape;

        public void AddEscFunction(Action function)
        {
            functionsForEscape.Enqueue(function);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EscapePressed();
            }
        }

        private void EscapePressed()
        {
            functionsForEscape.Dequeue()();
        }
    }

}