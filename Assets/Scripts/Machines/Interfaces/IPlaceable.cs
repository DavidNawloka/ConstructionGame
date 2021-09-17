using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public Vector2Int[] GetTakenGridPositions();
        public void SetTakenGridPositions(Vector2Int[] takenGridPositions);
        public InventoryItem[] GetNeededBuildingElements();
        public void SetOrigin(Vector2Int gridOrigin);
        public Vector2Int GetOrigin();
        public void FullyPlaced(Builder player);
        public void ChangeVersion();
    }
}