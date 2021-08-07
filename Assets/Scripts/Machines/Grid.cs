using UnityEngine;

public class Grid
{
    int width;
    int height;
    float cellSize;
    int[,] gridArray;
    Vector3 origin;
    public Grid(int width,int height,float cellSize, Vector3 origin)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        gridArray = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = 0;
            }
        }
    }
}
