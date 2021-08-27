using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public class BuildingGrid
    {
        int width;
        int height;
        float cellSize;
        Vector3 origin;

        Texture2D gridTexture;
        GridCell[,] gridArray;
        public BuildingGrid(int width, int height, float cellSize, Vector3 origin)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.origin = origin;

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
        }

        private void SetupGridCell(int x, int y)
        {
            Collider[] hitColliders = Physics.OverlapBox(
                                    GetWorldPositionCenter(x, y),
                                    new Vector3(cellSize / 2, cellSize/3, cellSize / 2),
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
            else if(foundGround && foundElement)
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
                    gridTexture.Apply();
                }
                else
                {
                    SetupGridCell(x, y);
                }
                gridArray[x, y].obstructed = obstructed;
            }
        }
        public void SetObstructed(Vector3 worldPosition, bool obstructed)
        {
            int x;
            int y;
            GetGridPosition(worldPosition, out x, out y);
            SetObstructed(x, y, obstructed);
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