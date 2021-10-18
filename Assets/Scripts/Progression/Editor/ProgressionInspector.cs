using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace CON.Progression 
{
    [CustomEditor(typeof(ProgressionManager))]
    public class ProgressionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ProgressionManager progressionManager = (ProgressionManager)target;

            if (GUILayout.Button("Instantiate Node Connectors"))
            {
                progressionManager.InstantiateConnectors();
            }
            if (GUILayout.Button("Delete all instantiated Node Connectors"))
            {
                progressionManager.DeleteConnectors();
            }
        }
    }
}