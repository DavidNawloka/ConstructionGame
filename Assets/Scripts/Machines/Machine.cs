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
        [SerializeField] Instruction[] possibleInstructions;
        [SerializeField] float productionIntervall;
        [SerializeField] Transform elementExitPoint;
        [Header("Unity Events")]
        [SerializeField] UnityEvent OnMachineClicked;

        Inventory inventory;
        bool fullyPlaced = false;
        Inventory playerInventory;
        Instruction currentInstruction;

        float productionTimer = 0f;
        int elementsProduced = 0;
        Vector2Int gridOrigin;

        void Awake()
        {
            inventory = GetComponent<Inventory>();
            currentInstruction = possibleInstructions[0];
        }

        private void Update()
        {
            if (!fullyPlaced || !inventory.HasItem(currentInstruction.requirements))
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
            if (playerInventory.HasItem(currentInstruction.requirements))
            {
                playerInventory.RemoveItem(currentInstruction.requirements);
                inventory.EquipItem(currentInstruction.requirements);
            }
        }
        public void AddEnergyElement(ElementPickup elementToAdd) // Event for when Elements enter
        {
            if (!CheckIfElementIsNeeded(elementToAdd)) return;
            Destroy(elementToAdd.gameObject);
            inventory.EquipItem(elementToAdd.GetItemToEquip());
        }
        public Instruction[] GetPossibleInstructions()
        {
            return possibleInstructions;
        }
        public Instruction GetCurrentInstruction()
        {
            return currentInstruction;
        }
        public void SetCurrentInstruction(Instruction currentInstruction)
        {
            this.currentInstruction = currentInstruction;
        }
        public float GetProductionFraction()
        {
            return productionTimer / productionIntervall;
        }

        private bool CheckIfElementIsNeeded(ElementPickup elementToAdd)
        {
            foreach(InventoryItem inventoryItem in currentInstruction.requirements)
            {
                if (elementToAdd.GetItemToEquip().element == inventoryItem.element) return true;
            }
            return false;
        }

        private void ProduceElement()
        {
            Instantiate(currentInstruction.outcome.element.pickupPrefab, elementExitPoint.position, Quaternion.identity);
            elementsProduced++;

            if (elementsProduced >= currentInstruction.outcome.amount)
            {
                inventory.RemoveItem(currentInstruction.requirements);
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

        public void ChangeVersion()
        {
            
        }
    }
}