using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Machines
{
    public class TunnelConveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [SerializeField] Transform elementExitPoint;
        [SerializeField] float elementExitForce = 2;
        [SerializeField] GameObject directionArrow;

        Vector2Int gridOrigin;
        Builder player;

        private void Start()
        {
            if (player != null) OnBuildModeChange(false);
        }

        private void OnDisable()
        {
            if (player == null) return;
            player.onBuildModeChange -= OnBuildModeChange;
        }

        private void OnCollisionEnter(Collision collision)
        {
            ElementPickup elementPickup = collision.transform.GetComponentInParent<ElementPickup>();

            if (elementPickup == null) return;

            Rigidbody rigidbody = elementPickup.transform.GetComponentInParent<Rigidbody>();
            rigidbody.isKinematic = true;
            elementPickup.transform.position = elementExitPoint.position;
            rigidbody.isKinematic = false;
            rigidbody.AddForce(-transform.right* elementExitForce, ForceMode.Impulse);

        }
        private Vector3 GetForceToKeepOnConveyor(Transform element)
        {
            if (Mathf.Approximately(transform.rotation.eulerAngles.y, 90) || Mathf.Approximately(transform.rotation.eulerAngles.y, 270))
            {
                return new Vector3(transform.position.x - element.position.x, 0, 0);
            }
            else
            {
                return new Vector3(0, 0, transform.position.z - element.position.z);
            }
        }
        private void OnBuildModeChange(bool isActive)
        {
            directionArrow.SetActive(isActive);
        }

        // Interface implementations
        public Vector2Int[] GetTakenGridPositions()
        {
            return takenGridPositions;
        }
        public void SetTakenGridPositions(Vector2Int[] takenGridPositions)
        {
            this.takenGridPositions = takenGridPositions;
        }
        public void FullyPlaced(Builder player)
        {
            //GetComponent<NavMeshObstacle>().enabled = true; TODO: Check if other possibility for more walkability
            this.player = player;
            player.onBuildModeChange += OnBuildModeChange;
        }

        public InventoryItem[] GetNeededBuildingElements()
        {
            return elementBuildingRequirements;
        }
        public void SetOrigin(Vector2Int gridOrigin)
        {
            this.gridOrigin = gridOrigin;
        }

        public Vector2Int GetOrigin()
        {
            return gridOrigin;
        }
        public void ChangeVersion()
        {

        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public object GetInformationToSave()
        {
            return null;
        }

        public void LoadSavedInformation(object savedInformation)
        {

        }

    }

}