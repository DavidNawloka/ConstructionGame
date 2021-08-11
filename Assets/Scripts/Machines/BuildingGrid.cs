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

        GridCell[,] gridArray;
        public BuildingGrid(int width, int height, float cellSize, Vector3 origin)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.origin = origin;

            gridArray = new GridCell[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    SetupGridCell(x, y);
                }
            }
            Debug.DrawLine(GetWorldPositionOrigin(0, height), GetWorldPositionOrigin(width, height), Color.white, 50f);
            Debug.DrawLine(GetWorldPositionOrigin(width, 0), GetWorldPositionOrigin(width, height), Color.white, 50);
        }

        private void SetupGridCell(int x, int y)
        {
            Collider[] hitColliders = Physics.OverlapBox(
                                    GetWorldPositionCenter(x, y),
                                    new Vector3(cellSize / 2, 20, cellSize / 2),
                                    Quaternion.identity, ~0, QueryTriggerInteraction.Collide);

            Color debugColor = Color.white;
            foreach (Collider collider in hitColliders)
            {
                ElementIndicator elementIndicator = collider.transform.GetComponent<ElementIndicator>();
                if (collider.transform.tag == "Mountain")
                {
                    gridArray[x, y] = new GridCell(true, null);
                    debugColor = Color.red;
                }
                else if (elementIndicator != null)
                {
                    gridArray[x, y] = new GridCell(false, elementIndicator.GetElement());
                    debugColor = elementIndicator.GetElement().colorRepresentation;
                }
                else
                {
                    gridArray[x, y] = new GridCell(false, null);
                }


            }
            Debug.DrawLine(GetWorldPositionOrigin(x, y), GetWorldPositionOrigin(x, y + 1), debugColor, 50f);
            Debug.DrawLine(GetWorldPositionOrigin(x, y), GetWorldPositionOrigin(x + 1, y), debugColor, 50f);
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