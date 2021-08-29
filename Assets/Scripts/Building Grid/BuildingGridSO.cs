using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.BuildingGrid
{
    [System.Serializable]
    public class BuildingGridSO : ScriptableObject
    {
        public GridCell[] gridArray;
        public BuildingGridSettings gridSettings;
        public void SaveGrid(GridCell[,] gridArrayMulti, BuildingGridSettings settings)
        {
            gridArray = new GridCell[settings.width * settings.height];
            int index = 0;
            foreach(GridCell gridCell in gridArrayMulti)
            {
                gridArray[index] = gridCell;
                index++;
            }

            gridSettings = settings;
        }
        public GridCell[,] LoadGrid()
        {
            int x = 0;
            int y = 0;
            GridCell[,] gridArrayMulti = new GridCell[gridSettings.width, gridSettings.height];
            foreach(GridCell gridCell in gridArray)
            {
                if(y == gridSettings.height)
                {
                    y = 0;
                    x++;
                }
                gridArrayMulti[x, y] = gridCell;

                y++;
            }
            return gridArrayMulti;
        }

        public BuildingGridSettings GetSettings()
        {
            return gridSettings;
        }

    }
    [System.Serializable]
    public class BuildingGridSettings
    {
        public int width;
        public int height;
        public float cellSize;
        public Vector3 origin;
        public BuildingGridSettings(int width, int height, float cellSize, Vector3 origin)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.origin = origin;
        }
    }
}