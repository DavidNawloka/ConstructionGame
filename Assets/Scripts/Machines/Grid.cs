using CON.Elements;
using UnityEngine;

public class Grid
{
    int width;
    int height;
    float cellSize;
    Vector3 origin;

    GridCell[,] gridArray;
    public Grid(int width,int height,float cellSize, Vector3 origin)
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
                Collider[] hitColliders = Physics.OverlapBox(
                    GetWorldPosition(x, y) + new Vector3(cellSize / 2, 0, cellSize / 2), 
                    new Vector3(cellSize / 2, cellSize / 2, cellSize / 2), 
                    Quaternion.identity, ~0, QueryTriggerInteraction.Collide);

                Color debugColor = Color.white;
                foreach(Collider collider in hitColliders)
                {
                    ElementIndicator elementIndicator = collider.transform.GetComponent<ElementIndicator>();
                    if (elementIndicator != null)
                    {
                        gridArray[x, y] = new GridCell(false, elementIndicator.GetElement());
                        debugColor = elementIndicator.GetElement().colorRepresentation;
                    }
                    else if(collider.transform.tag == "Mountain")
                    {
                        gridArray[x, y] = new GridCell(true, null);
                        debugColor = Color.red;
                    }
                    else
                    {
                        gridArray[x, y] = new GridCell(false, null);
                    }

                    
                }
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), debugColor, 50f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x+1, y), debugColor, 50f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width,height), Color.white, 50f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 50);
    }
    public bool IsObstructed(int x,int y)
    {
        return gridArray[x, y].IsObstructed();
    }
    public bool HasElement(int x, int y, Element element)
    {
        return gridArray[x, y].GetElement() == element;
    }
    public void SetValue(int x, int y, bool obstructed)
    {
        if(x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y].UpdateObstructed(obstructed);
        }
    }
    public void SetValue(Vector3 worldPosition, bool obstructed)
    {
        int x;
        int y;
        GetGridPosition(worldPosition, out x ,out y);
        SetValue(x,y, obstructed);
    }

    public Vector3 GetNearestGridWorldPosition(Vector3 worldPosition)
    {
        int x;
        int y;
        GetGridPosition(worldPosition,out x,out y);
        return GetWorldPosition(x, y) + new Vector3(cellSize/2,0,cellSize/2);
    }
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,0,y)*cellSize + origin;
    }

    public void GetGridPosition(Vector3 worldPosition,out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - origin).z / cellSize);
    }

}

public class GridCell
{
    bool obstructed;
    Element element;
    public GridCell(bool obstructed, Element element)
    {
        this.obstructed = obstructed;
        this.element = element;
    }

    public void UpdateObstructed(bool obstructed)
    {
        this.obstructed = obstructed;
    }
    public bool IsObstructed()
    {
        return obstructed;
    }
    public Element GetElement()
    {
        return element;
    }
}