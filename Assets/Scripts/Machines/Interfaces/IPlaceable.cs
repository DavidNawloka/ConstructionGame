using CON.Core;
using CON.Elements;
using UnityEngine;

namespace CON.Machines
{
    public interface IPlaceable
    {
        public PlaceableInformation GetPlaceableInformation();
        public void PlacementStatusChange(Builder player, PlacementStatus placementStatus);
        public void ChangeVersion();
        public bool IsFullyPlaced();
        public object GetInformationToSave();
        public void LoadSavedInformation(object savedInformation);
    }

    [System.Serializable]
    public class PlaceableInformation
    {
        public bool isDraggable = false;
        public Vector2Int[] takenGridPositions;
        public InventoryItem[] buildingRequirements;
        public Element placementRequirement;
        public ColoredPlaceable[] coloredPlaceables;
        public AudioSourceManager audioSourceManager;
        public ParticleSystem placementParticles;
        public Vector2Int buildingGridOrigin;
        public string uniqueIdentifier;
    }
    [System.Serializable]
    public class ColoredPlaceable
    {
        public Color color;
        public GameObject gameObject;
    }
    public enum PlacementStatus
    {
        startingPlacement,
        endingPlacement,
        startingDemolishment,
        endingDemolishment
    }
}