using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CON.Elements;
using System;

namespace CON.BuildingGrid
{
    public class BuildingGridWindow : EditorWindow
    {
        int width;
        int height;
        float cellSize;
        Vector3 origin;

        [MenuItem("Window/Building Grid")]
        public static void ShowWindow()
        {
            GetWindow<BuildingGridWindow>(false, "Building Grid", true);
        }
        private void OnEnable()
        {
            LoadBuildingGridSettings();
        }
        public void OnGUI()
        {
            GUILayout.Label("Building Grid Settings");
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            cellSize = EditorGUILayout.FloatField("Cell Size", cellSize);
            origin = EditorGUILayout.Vector3Field("Origin", origin);

            if (GUILayout.Button("Save Building Grid, Texture and Mesh"))
            {
                BuildGrid();
            }
            if (GUILayout.Button("Delete Building Grid, Texture and Mesh"))
            {
                BuildingGridAssetManager.DeleteAllData();
            }
            if (GUILayout.Button("Load Building Grid Settings"))
            {
                LoadBuildingGridSettings();
            }

        }

        private void LoadBuildingGridSettings()
        {
            BuildingGridSettings settings = BuildingGridAssetManager.LoadSettings();
            if (settings == null) return;
            width = settings.width;
            height = settings.height;
            cellSize = settings.cellSize;
            origin = settings.origin;
        }


        Texture2D gridTexture;
        GridCell[,] gridArray;
        private void BuildGrid()
        {
            gridTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            gridTexture.filterMode = FilterMode.Point;
            gridArray = new GridCell[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    SetupGridCell(x, y);
                }
            }

            BuildingGridSettings buildingGridSettings = new BuildingGridSettings(width, height, cellSize, origin);

            BuildingGridAssetManager.SaveGrid(gridArray, buildingGridSettings);
            BuildingGridAssetManager.SaveTexture(gridTexture);
            BuildingGridAssetManager.SaveMesh(buildingGridSettings);
        }

        private void SetupGridCell(int x, int y)
        {
            Collider[] hitColliders = Physics.OverlapBox(
                                    new Vector3(x, 0, y) * cellSize + origin + new Vector3(cellSize / 2, 0, cellSize / 2),
                                    new Vector3(cellSize / 2, cellSize / 3, cellSize / 2),
                                    Quaternion.identity, ~0, QueryTriggerInteraction.Collide);

            bool foundElement = false;
            bool foundGround = false;
            bool foundMountain = false;
            ElementIndicator elementIndicator = null;
            foreach (Collider collider in hitColliders)
            {
                ElementIndicator temporaryElementIndicator = collider.transform.GetComponent<ElementIndicator>();
                if (collider.transform.tag == "Mountain")
                {
                    foundMountain = true;
                }
                if (collider.transform.tag == "Ground")
                {
                    foundGround = true;
                }
                if (temporaryElementIndicator != null)
                {
                    elementIndicator = temporaryElementIndicator;
                    foundElement = true;
                }

            }
            if (foundMountain)
            {
                gridArray[x, y] = new GridCell(true, null);
                gridTexture.SetPixel(x, y, Color.red);
            }
            else if (foundGround && foundElement)
            {

                gridArray[x, y] = new GridCell(false, elementIndicator.GetElement());
                gridTexture.SetPixel(x, y, elementIndicator.GetElement().colorRepresentation);
            }
            else if (foundGround)
            {
                gridArray[x, y] = new GridCell(false, null);
                gridTexture.SetPixel(x, y, Color.white);
            }
            else
            {
                gridArray[x, y] = new GridCell(true, null);
                gridTexture.SetPixel(x, y, Color.red);
            }

            gridTexture.Apply();
        }
        
    }
}