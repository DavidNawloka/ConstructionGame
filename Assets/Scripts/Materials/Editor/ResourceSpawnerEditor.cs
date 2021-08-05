using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CON.Materials
{
    [CustomEditor(typeof(MaterialSpawner))]
    public class MaterialSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MaterialSpawner resourceSpawner = (MaterialSpawner)target;

            if (GUILayout.Button("Spawn resources"))
            {
                resourceSpawner.SpawnResources();
            }
            if (GUILayout.Button("Delete all spawned resources"))
            {
                resourceSpawner.DeleteResources();
            }
        }
    }
}