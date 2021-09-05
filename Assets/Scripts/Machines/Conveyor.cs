using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Machines
{
    public class Conveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [SerializeField] Transform[] pathOfElement;
        [SerializeField] float forceToApplyForward;
        [SerializeField] float forceToApplySide;
        [SerializeField] GameObject directionArrow;

        Vector2Int gridOrigin;
        Builder player;

        private void OnDisable()
        {
            if (player == null) return;
            player.onBuildModeChange -= OnBuildModeChange;
        }

        private void OnCollisionStay(Collision collision)
        {
            ElementPickup elementPickup = collision.transform.GetComponentInParent<ElementPickup>();
            if (elementPickup == null) return;

            Rigidbody rigidbody = collision.transform.GetComponentInParent<Rigidbody>();

            rigidbody.velocity = (pathOfElement[1].position -pathOfElement[0].position).normalized * forceToApplyForward;
            rigidbody.AddForce(GetForceToKeepOnConveyor(rigidbody.transform)* forceToApplySide, ForceMode.Impulse);
        }

        private Vector3 GetForceToKeepOnConveyor(Transform element)
        {
            if(Mathf.Approximately(transform.rotation.eulerAngles.y,90) || Mathf.Approximately(transform.rotation.eulerAngles.y, 270))
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
        public void FullyPlaced(Builder player)
        {
            //GetComponent<NavMeshObstacle>().enabled = true; TODO: Check if other possibility for more walkability
            GetComponent<BoxCollider>().enabled = true;
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
    }

}