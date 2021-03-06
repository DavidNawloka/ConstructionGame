using CON.Core;
using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines
{
    public class TunnelConveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] PlaceableInformation placeableInformation;
        [SerializeField] Transform elementExitPoint;
        [SerializeField] float elementExitForce = 2;
        [SerializeField] Transform[] elementExitConveyors;
        [SerializeField] BoxCollider elementExitCollider;
        [SerializeField] Transform elementEntryConveyor;
        [SerializeField] int maxTunnelBlocks = 3;
        [SerializeField] GameObject directionArrow;
        [SerializeField] AudioClip[] conveyorSounds;

        Builder player;
        AudioSourceManager audioLoop;
        bool isFullyPlaced = false;

        Vector3 initialLocalElementExitLocation;
        int tunnelAdditionalBlocks = 0;

        private void Awake()
        {
            audioLoop = GetComponent<AudioSourceManager>();
            initialLocalElementExitLocation = elementExitConveyors[0].localPosition;
        }
        private void Start()
        {
            if (player != null) OnBuildModeChange(false);
        }

        private void OnDisable()
        {
            if (player == null) return;
            player.onBuildModeChange -= OnBuildModeChange;
        }
        private void OnTriggerEnter(Collider other)
        {
            ElementPickup elementPickup = other.transform.GetComponentInParent<ElementPickup>();

            if (elementPickup == null || !isFullyPlaced) return;

            Rigidbody rigidbody = elementPickup.transform.GetComponentInParent<Rigidbody>();
            rigidbody.isKinematic = true;
            elementPickup.transform.position = elementExitPoint.position;
            rigidbody.isKinematic = false;
            rigidbody.AddForce(-transform.right* elementExitForce, ForceMode.Impulse);

        }
        private void OnBuildModeChange(bool isActive)
        {
            directionArrow.SetActive(isActive);
        }

        private void UpdateExitLocation()
        {
            foreach(Transform exitConveyor in elementExitConveyors)
            {
                exitConveyor.localPosition = new Vector3(initialLocalElementExitLocation.x - 1.5f * tunnelAdditionalBlocks, initialLocalElementExitLocation.y, initialLocalElementExitLocation.z);
            }
            elementExitCollider.center = elementExitConveyors[0].localPosition;
            placeableInformation.takenGridPositions[1] = Vector2Int.RoundToInt(((Vector2)placeableInformation.takenGridPositions[1]).normalized * (tunnelAdditionalBlocks+2));
            Vector3 arrowLocation = (elementEntryConveyor.position + elementExitConveyors[0].position) / 2;
            directionArrow.transform.position = new Vector3(arrowLocation.x,directionArrow.transform.position.y,arrowLocation.z);
        }

        // Interface implementations
        public void PlacementStatusChange(Builder player, PlacementStatus placementStatus)
        {
            switch (placementStatus)
            {
                case PlacementStatus.startingPlacement:
                    foreach (NavMeshObstacle obstacle in GetComponentsInChildren<NavMeshObstacle>())
                    {
                        obstacle.enabled = true;
                    }
                    player.onBuildModeChange += OnBuildModeChange;
                    foreach(BoxCollider boxCollider in GetComponents<BoxCollider>())
                    {
                        boxCollider.enabled = true;
                    }
                    this.player = player;
                    break;
                case PlacementStatus.endingPlacement:
                    isFullyPlaced = true;
                    audioLoop.StartLooping(conveyorSounds);
                    break;
                case PlacementStatus.startingDemolishment:
                    isFullyPlaced = false;
                    break;
                case PlacementStatus.endingDemolishment:
                    Destroy(gameObject);
                    break;
            }
        }

        public bool IsFullyPlaced()
        {
            return isFullyPlaced;
        }
        public void ChangeVersion()
        {
            tunnelAdditionalBlocks++;
            if (tunnelAdditionalBlocks == maxTunnelBlocks) tunnelAdditionalBlocks = 0;
            UpdateExitLocation();

        }
        public PlaceableInformation GetPlaceableInformation()
        {
            return placeableInformation;
        }
        public object GetInformationToSave()
        {
            return tunnelAdditionalBlocks;
        }

        public void LoadSavedInformation(object savedInformation)
        {
            if (savedInformation == null) return;
            tunnelAdditionalBlocks = (int)savedInformation;
            UpdateExitLocation();
        }

    }

}