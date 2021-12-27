using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CON.Progression
{
    [CustomEditor(typeof(FindableNote))][CanEditMultipleObjects]
    public class FindableNoteInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            FindableNote findableNote = (FindableNote)target;

            if (GUILayout.Button("Update Perfect Key Distances"))
            {
                findableNote.UpdatePerfectKeyDistances();
            }
        }
    }
}