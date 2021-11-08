using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public PlaceableInformation GetPlaceableInformation();
        public void StartingPlacement(Builder player);
        public void FullyPlaced(Builder player);
        public void ChangeColor(Color color);
        public void ChangeVersion();
        public GameObject GetGameObject();
        public object GetInformationToSave();
        public void LoadSavedInformation(object savedInformation);
    }

    [System.Serializable]
    public class PlaceableInformation
    {
        public Vector2Int[] takenGridPositions;
        public InventoryItem[] buildingRequirements;
        public Element placementRequirement;
        public GameObject greenPlaceable;
        public GameObject redPlaceable;
        public GameObject normalPlaceable;
        public Vector2Int buildingGridOrigin;
        public string uniqueIdentifier;
    }
}