using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CON.BuildingGrid
{
    [CustomEditor(typeof(BuildingGridMesh))]
    public class BuildingGridMeshInspector : Editor
    {
        bool isShown = true;
        public override void OnInspectorGUI()
        {
            BuildingGridMesh buildingGridMesh = (BuildingGridMesh)target;

            DrawDefaultInspector();
            if(GUILayout.Button("Toggle Mesh"))
            {
                Texture2D gridTexture = Resources.Load<Texture2D>(BuildingGridAssetManager.GetTextureName());
                if (gridTexture == null) return;

                buildingGridMesh.InitiatePlane(gridTexture);
                buildingGridMesh.SetActiveMesh(!isShown);
                isShown = !isShown;
            }
        }
    }

}