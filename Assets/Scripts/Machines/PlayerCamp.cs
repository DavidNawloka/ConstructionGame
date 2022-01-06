using CON.Core;
using CON.Player;
using CON.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines
{
    public class PlayerCamp : MonoBehaviour, IPlaceable, IRaycastable
    {
        [SerializeField] PlaceableInformation placeableInformation;
        [SerializeField] Transform navMeshObstaclesParent;
        [SerializeField] ParticleSystem campFireParticleSystem;

        bool fullyPlaced = false;
        UserInterfaceManager userInterfaceManager;
        Builder builder;

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
                    campFireParticleSystem.Play();
                    builder = player;
                    foreach (Transform child in navMeshObstaclesParent)
                    {
                        child.GetComponent<NavMeshObstacle>().enabled = true;
                    }
                    break;
                case PlacementStatus.endingDemolishment:
                    Destroy(gameObject);
                    break;
                case PlacementStatus.endingPlacement:
                    GetComponent<AudioSource>().Play();
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
            return fullyPlaced && !builder.IsDemolishMode();
        }

        public void HandleInteractionClick(Transform player)
        {
            if (EventSystem.current.IsPointerOverGameObject() || builder.IsDemolishMode()) return;

            userInterfaceManager.ToggleUI(3);
        }
    }

}