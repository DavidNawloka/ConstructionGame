using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CON.Elements
{
    [CustomEditor(typeof(ElementSpawner))]
    public class ElementSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ElementSpawner resourceSpawner = (ElementSpawner)target;

            if (GUILayout.Button("Spawn element pickups"))
            {
                resourceSpawner.SpawnResources();
            }
            if (GUILayout.Button("Delete all spawned element pickups"))
            {
                resourceSpawner.DeleteResources();
            }
        }
    }
}