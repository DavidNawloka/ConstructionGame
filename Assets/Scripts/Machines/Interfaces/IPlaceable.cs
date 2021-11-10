using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public PlaceableInformation GetPlaceableInformation();
        public void PlacementStatusChange(Builder player, bool isBeginning);
        public void ChangeVersion();
        public object GetInformationToSave();
        public void LoadSavedInformation(object savedInformation);
    }

    [System.Serializable]
    public class PlaceableInformation
    {
        public Vector2Int[] takenGridPositions;
        public InventoryItem[] buildingRequirements;
        public Element placementRequirement;
        public ColoredPlaceable[] coloredPlaceables;
        public Vector2Int buildingGridOrigin;
        public string uniqueIdentifier;
    }
    [System.Serializable]
    public class ColoredPlaceable
    {
        public Color color;
        public GameObject gameObject;
    }
}