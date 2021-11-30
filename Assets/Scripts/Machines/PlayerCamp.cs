using CON.Core;
using CON.Player;
using CON.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines
{
    public class PlayerCamp : MonoBehaviour, IPlaceable, IRaycastable
    {
        [SerializeField] PlaceableInformation placeableInformation;
        [SerializeField] Transform navMeshObstaclesParent;


        bool fullyPlaced = false;
        UserInterfaceManager userInterfaceManager;

        private void Awake()
        {
            userInterfaceManager = FindObjectOfType<UserInterfaceManager>();
        }

        public bool IsFullyPlaced()
        {
            return fullyPlaced;
        }
        public PlaceableInformation GetPlaceableInformation()
        {
            return placeableInformation;
        }

        public void PlacementStatusChange(Builder player, PlacementStatus placementStatus)
        {
            switch (placementStatus)
            {
                case PlacementStatus.startingPlacement:
                    foreach (Transform child in navMeshObstaclesParent)
                    {
                        child.GetComponent<NavMeshObstacle>().enabled = true;
                    }
                    break;
                case PlacementStatus.endingDemolishment:
                    Destroy(gameObject);
                    break;
                case PlacementStatus.endingPlacement:
                    fullyPlaced = true;
                    break;
            }
        }
        public void ChangeVersion()
        {

        }
        public object GetInformationToSave()
        {
            return null;
        }
        public void LoadSavedInformation(object savedInformation)
        {
            
        }

        public CursorType GetCursorType()
        {
            return CursorType.Camp;
        }

        public bool InRange(Transform player)
        {
            return fullyPlaced;
        }

        public void HandleInteractionClick(Transform player)
        {
            userInterfaceManager.ToggleUI(3);
        }
    }

}