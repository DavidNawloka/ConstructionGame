using CON.Elements;
using UnityEngine;

namespace CON.BuildingGrid
{
    public class BuildingGridManager
    {
        int width;
        int height;
        float cellSize;
        Vector3 origin;

        Texture2D gridTexture;
        public GridCell[,] gridArray;

        public BuildingGridManager(GridCell[,] gridArray, Texture2D gridTexture)
        {
            this.gridArray = gridArray;
            BuildingGridAssetManager.LoadSettings(out width, out height, out cellSize, out origin);
            this.gridTexture = gridTexture;
        }
        
        public Texture2D GetBuildingGridTexture()
        {
            return gridTexture;
        }

        public Vector3 GetWorldPositionOrigin(int x, int y)
        {
            return new Vector3(x, 0, y) * cellSize + origin;
        }
        public Vector3 GetWorldPositionCenter(int x, int y)
        {
            return GetWorldPositionOrigin(x, y) + new Vector3(cellSize / 2, 0, cellSize / 2);
        }

        public void GetGridPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - origin).z / cellSize);
        }
        public void SetObstructed(int x, int y, bool obstructed)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                if (obstructed)
                {
                    gridTexture.SetPixel(x, y, Color.red);
                }
                else
                {
                    if(gridArray[x,y].element == null)
                    {
                        gridTexture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        gridTexture.SetPixel(x, y, gridArray[x, y].element.colorRepresentation);
                    }
                }
                gridArray[x, y].obstructed = obstructed;
                gridTexture.Apply();
            }
        }
        public void SetObstructed(Vector3 worldPosition, bool obstructed)
        {
            int x;
            int y;
            GetGridPosition(worldPosition, out x, out y);
            SetObstructed(x, y, obstructed);
        }
        public Vector3 GetOrigin()
        {
            return origin;
        }
        public bool IsObstructed(int x, int y)
        {
            return gridArray[x, y].obstructed;
        }
        public bool HasElement(int x, int y, Element element)
        {
            return gridArray[x, y].element == element;
        }
    }

    [System.Serializable]
    public class GridCell
    {
        public bool obstructed;
        public Element element;
        public GridCell(bool obstructed, Element element)
        {
            this.obstructed = obstructed;
            this.element = element;
        }
    }
}