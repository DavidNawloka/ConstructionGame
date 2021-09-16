using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CON.BuildingGrid
{
    public static class BuildingGridAssetManager
    {
        static readonly string PATH = "Assets/Scripts/Building Grid/Resources/";
        static readonly string SO_NAME = "BuildingGrid";
        static readonly string TEXTURE_NAME = "BuildingGridTexture";
        static readonly string MESH_NAME = "BuildingGridMesh";
        static readonly string EXTENSION = ".asset";


#if UNITY_EDITOR
        public static void SaveGrid(GridCell[,] gridArray, BuildingGridSettings buildingGridSettings)
        {
            DeleteAllData();

            BuildingGridSO instance;

            instance = AssetDatabase.LoadAssetAtPath<BuildingGridSO>(PATH + SO_NAME + EXTENSION);

            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<BuildingGridSO>();
                AssetDatabase.CreateAsset(instance, PATH + SO_NAME + EXTENSION);
            }
            instance.SaveGrid(gridArray, buildingGridSettings);

            EditorUtility.SetDirty(instance); // Without it data is lost after Unity restart

            AssetDatabase.SaveAssets();
        }
        public static void SaveTexture(Texture2D gridTexture)
        {
            AssetDatabase.CreateAsset(gridTexture, PATH + TEXTURE_NAME + EXTENSION);
            AssetDatabase.SaveAssets();
        }
        public static void SaveMesh(BuildingGridSettings buildingGridSettings)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
            new Vector3(0, 0, 0),
            new Vector3(buildingGridSettings.width, 0, 0),
            new Vector3(0, 0, buildingGridSettings.height),
            new Vector3(buildingGridSettings.width, 0, buildingGridSettings.height),
            };
            Vector2[] uv = new Vector2[4]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            };
            int[] tris = new int[6]
            {
            0, 2, 1,
            2, 3, 1
            };
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = tris;

            mesh.RecalculateNormals();

            AssetDatabase.CreateAsset(mesh, PATH + MESH_NAME + EXTENSION);
            AssetDatabase.SaveAssets();
        }
        

        public static void DeleteAllData()
        {
            AssetDatabase.DeleteAsset(PATH + SO_NAME + EXTENSION);
            AssetDatabase.DeleteAsset(PATH + TEXTURE_NAME + EXTENSION);
            AssetDatabase.DeleteAsset(PATH + MESH_NAME + EXTENSION);
        }
#endif

        public static BuildingGridSettings LoadSettings()
        {
            BuildingGridSO buildingGrid = Resources.Load<BuildingGridSO>(SO_NAME);
            if (buildingGrid == null) return null;

            return buildingGrid.GetSettings();
        }
        public static void LoadSettings(out int width, out int height, out float cellSize, out Vector3 origin)
        {
            BuildingGridSO buildingGrid = Resources.Load<BuildingGridSO>(SO_NAME);

            width = buildingGrid.GetSettings().width;
            height = buildingGrid.GetSettings().height;
            cellSize = buildingGrid.GetSettings().cellSize;
            origin = buildingGrid.GetSettings().origin;
        }

        public static string GetTextureName()
        {
            return TEXTURE_NAME;
        }
        public static GridCell[,] GetGrid()
        {
            BuildingGridSO buildingGrid = Resources.Load<BuildingGridSO>(SO_NAME);
            return buildingGrid.LoadGrid();
        }
        public static void GetGridAndSettings(out GridCell[,] gridArray, out int width, out int height, out float cellSize, out Vector3 origin)
        {
            BuildingGridSO buildingGrid = Resources.Load<BuildingGridSO>(SO_NAME);
            gridArray = buildingGrid.LoadGrid();

            BuildingGridSettings buildingGridSettings = buildingGrid.GetSettings();
            width = buildingGridSettings.width;
            height = buildingGridSettings.height;
            cellSize = buildingGridSettings.cellSize;
            origin = buildingGridSettings.origin;
        }
        public static Texture2D GetGridTexture()
        {
            return Resources.Load<Texture2D>(TEXTURE_NAME);
        }
        public static Mesh GetGridMesh()
        {
            return Resources.Load<Mesh>(MESH_NAME);
        }
    }

}