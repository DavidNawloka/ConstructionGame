using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public Vector2Int[] GetTakenGridPositions();
        public void FullyPlaced();
    }
}