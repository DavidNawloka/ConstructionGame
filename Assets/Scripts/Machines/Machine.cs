using CON.Elements;
using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using CON.Progression;

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
        [SerializeField] Transform elementExitPoint;
        [SerializeField] ElementTrigger[] elementTriggers;


        [HideInInspector] public event Action OnMachineClicked;

        Inventory inventory;
        bool isPaused = false;
        bool fullyPlaced = false;
        Inventory playerInventory;
        Builder builder;
        Instruction currentInstruction;
        ProgressionManager progressionManager;

        float productionTimer = 0f;
        Vector2Int gridOrigin;

        void Awake()
        {
            inventory = GetComponent<Inventory>();
            progressionManager = FindObjectOfType<ProgressionManager>();
            currentInstruction = possibleInstructions[0];
        }
        private void Start()
        {
            UpdateElementTriggers();
        }
        private void Update()
        {
            if (isPaused || !fullyPlaced || !inventory.HasItem(currentInstruction.requirements))
            {
                ToggleAnimations(false);
                return;
            }

            if (productionTimer >= currentInstruction.productionInterval)
            {
                productionTimer = 0;
                ProduceElement();
            }


            ToggleAnimations(true);
            productionTimer += Time.deltaTime;
        }

        public void SetPause(bool isPaused)
        {
            this.isPaused = isPaused;
            productionTimer = 0;
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

            inventory.EquipItemAt(elementToAdd.GetItemToEquip(), GetElementInstructionIndex(elementToAdd.GetItemToEquip().element));
        }


        public bool GetFullyPlacedStatus()
        {
            return fullyPlaced;
        }
        public Instruction[] GetPossibleInstructions()
        {
            return possibleInstructions;
        }
        public Instruction GetCurrentInstruction()
        {
            return currentInstruction;
        }
        public int GetCurrentInstructionIndex()
        {
            for (int index = 0; index < possibleInstructions.Length; index++)
            {
                if(currentInstruction == possibleInstructions[index])
                {
                    return index;
                }
            }
            return 0;
        }
        public void SetCurrentInstruction(int instructionIndex)
        {
            currentInstruction = possibleInstructions[instructionIndex];
            UpdateElementTriggers();
        }
        public void SetCurrentInstruction(Instruction instruction)
        {
            currentInstruction = instruction;
            UpdateElementTriggers();
        }
        public float GetProductionFraction()
        {
            return productionTimer / currentInstruction.productionInterval;
        }

        private int GetElementInstructionIndex(Element element)
        {
            for (int index = 0; index < currentInstruction.requirements.Length; index++)
            {
                if (currentInstruction.requirements[index].element == element) return index;
            }
            return -1;
        }
        private void UpdateElementTriggers()
        {
            for (int requirementIndex = 0; requirementIndex < currentInstruction.requirements.Length; requirementIndex++)
            {
                elementTriggers[requirementIndex].UpdateFilter(currentInstruction.requirements[requirementIndex].element);
            }
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
            progressionManager.GetInventory().EquipItem(currentInstruction.outcome);

            for (int amount = 0; amount < currentInstruction.outcome.amount; amount++)
            {
                Instantiate(currentInstruction.outcome.element.pickupPrefab, elementExitPoint.position+ UnityEngine.Random.insideUnitSphere*.1f, Quaternion.identity);
                
            }
            inventory.RemoveItem(currentInstruction.requirements);

        }


        private void ToggleAnimations(bool isActive)
        {
            foreach(Animation animation in GetComponentsInChildren<Animation>())
            {
                AudioSource audioSource = animation.transform.GetComponent<AudioSource>();
                animation.playAutomatically = false;
                if (audioSource != null) audioSource.enabled = isActive;
                if (isActive)
                {
                    animation.Play();
                    
                }
                else
                {
                    animation.Rewind();
                    animation.Stop();
                }

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
        public void SetTakenGridPositions(Vector2Int[] takenGridPositions)
        {
            this.takenGridPositions = takenGridPositions;
        }
        public void FullyPlaced(Builder player)
        {
            GetComponent<NavMeshObstacle>().enabled = true;
            builder = player;
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
            if (!fullyPlaced || EventSystem.current.IsPointerOverGameObject() || builder.IsDemolishMode()) return;

            OnMachineClicked();
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
            return new SavedMachine(isPaused,productionTimer,currentInstruction,inventory.CaptureState());
        }

        public void LoadSavedInformation(object savedInformation)
        {
            SavedMachine savedMachine = (SavedMachine)savedInformation;

            productionTimer = savedMachine.productionTimer;
            SetCurrentInstruction(savedMachine.machineInstruction.DeSerialize());
            inventory.RestoreState(savedMachine.inventoryState);
            SetPause(savedMachine.isPaused);
        }


        [System.Serializable]
        private class SavedMachine
        {
            public bool isPaused;
            public float productionTimer;
            public SerializedInstruction machineInstruction;
            public object inventoryState;
            public SavedMachine(bool isPaused, float productionTimer, Instruction currentInstruction, object inventoryState)
            {
                this.productionTimer = productionTimer;
                this.machineInstruction = currentInstruction.Serialize();
                this.inventoryState = inventoryState;
                this.isPaused = isPaused;
            }
        }
        

    }
}