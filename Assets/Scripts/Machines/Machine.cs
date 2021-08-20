using CON.Elements;
using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;

namespace CON.Machines 
{
    public class Machine : MonoBehaviour, IPlaceable, IMouseClickable
    {
        [Header("Placement")]
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] Element elementPlacementRequirement;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [Header("Production")]
        [SerializeField] ElementPickup elementToProduce;
        [SerializeField] float productionIntervall;
        [SerializeField] Transform elementExitPoint;
        [SerializeField] InventoryItem energyRequirement;
        [SerializeField] int elementPerEnergy = 1;
        [Header("Unity Events")]
        [SerializeField] UnityEvent OnMachineClicked;

        Inventory inventory;
        bool fullyPlaced = false;

        float productionTimer = 0f;
        int elementsProduced = 0;

        void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            if (!fullyPlaced || !inventory.HasItem(energyRequirement)) return;

            if(productionTimer >= productionIntervall)
            {
                productionTimer = 0;
                ProduceElement();
            }

            productionTimer += Time.deltaTime;
        }

        private void ProduceElement()
        {
            Instantiate(elementToProduce.gameObject, elementExitPoint.position, Quaternion.identity);
            elementsProduced++;

            if (elementsProduced >= elementPerEnergy)
            {
                inventory.RemoveItem(energyRequirement);
                elementsProduced = 0;
            }
            
        }

        public void AddEnergyElement()
        {
            Inventory playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            if (playerInventory.HasItem(energyRequirement))
            {
                playerInventory.RemoveItem(energyRequirement);
                inventory.EquipItem(energyRequirement);
            }
        }

        public float GetProductionFraction()
        {
            return productionTimer / productionIntervall;
        }

        public InventoryItem GetEnergyRequirement()
        {
            return energyRequirement;
        }

        public Element GetElementPlacementRequirement()
        {
            return elementPlacementRequirement;
        }
        public Vector2Int[] GetTakenGridPositions()
        {
            return takenGridPositions;
        }
        public void FullyPlaced()
        {
            GetComponent<NavMeshObstacle>().enabled = true;
            fullyPlaced = true;
        }

        public InventoryItem[] GetNeededBuildingElements()
        {
            return elementBuildingRequirements;
        }

        public bool HandleInteractionClick(Transform player)
        {
            if (!fullyPlaced) return false;

            OnMachineClicked.Invoke();
            return false;
        }
    }
}