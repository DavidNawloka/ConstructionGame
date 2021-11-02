using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public PlaceableInformation GetPlaceableInformation();
        public void FullyPlaced(Builder player);
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
        public Vector2Int buildingGridOrigin;
        public string uniqueIdentifier;
    }
}