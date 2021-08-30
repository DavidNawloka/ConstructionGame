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
    public class Machine : MonoBehaviour, IPlaceable
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
        Inventory playerInventory;

        float productionTimer = 0f;
        int elementsProduced = 0;
        Vector2Int gridOrigin;

        void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            if (!fullyPlaced || !inventory.HasItem(energyRequirement))
            {
                ToggleAnimations(false);
                return;
            }

            if (productionTimer >= productionIntervall)
            {
                productionTimer = 0;
                ProduceElement();
            }


            ToggleAnimations(true);
            productionTimer += Time.deltaTime;
        }


        public void AddAllEnergyElements()
        {
            if (playerInventory.HasItem(energyRequirement))
            {
                playerInventory.RemoveItem(energyRequirement);
                inventory.EquipItem(energyRequirement);
            }
        }
        public void AddEnergyElement(InventoryItem elementToAdd) // Event for when Elements enter
        {
            if (elementToAdd.element != energyRequirement.element) return;
            inventory.EquipItem(elementToAdd);
        }

        public InventoryItem GetEnergyRequirement()
        {
            return energyRequirement;
        }
        public float GetProductionFraction()
        {
            return productionTimer / productionIntervall;
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


        private void ToggleAnimations(bool isActive)
        {
            foreach(Animation animation in GetComponentsInChildren<Animation>())
            {
                animation.playAutomatically = false;
                if (isActive) animation.Play();
                else animation.Stop();
            }
        }

        // Interface implementations


        public Element GetElementPlacementRequirement()
        {
            return elementPlacementRequirement;
        }
        public Vector2Int[] GetTakenGridPositions()
        {
            return takenGridPositions;
        }
        public void FullyPlaced(Builder player)
        {
            GetComponent<NavMeshObstacle>().enabled = true;
            fullyPlaced = true;
            playerInventory = player.GetComponent<Inventory>();
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
        private void OnMouseDown()
        {
            if (!fullyPlaced) return ;

            OnMachineClicked.Invoke();
        }

        
    }
}