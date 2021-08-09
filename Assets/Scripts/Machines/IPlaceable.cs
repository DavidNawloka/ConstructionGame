using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public Vector2Int[] GetTakenGridPositions();
        public InventoryItem[] GetNeededBuildingElements();
        public void FullyPlaced();
    }
}