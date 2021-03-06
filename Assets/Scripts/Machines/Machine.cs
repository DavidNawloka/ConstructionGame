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
using CON.Player;

namespace CON.Machines 
{
    public class Machine : MonoBehaviour, IPlaceable, IRaycastable
    {
        [Header("Placement")]
        [SerializeField] PlaceableInformation placeableInformation;
        [Header("Production")]
        [SerializeField] Instruction[] possibleInstructions;
        [SerializeField] Transform elementExitPoint;
        [SerializeField] ElementTrigger[] elementTriggers;


        [HideInInspector] public event Action OnMachineClicked;
        [HideInInspector] public event Action OnInstructionChanged;

        Inventory inventory;
        bool isPaused = false;
        bool fullyPlaced = false;
        Inventory playerInventory;
        Builder builder;
        Instruction currentInstruction;
        ProgressionManager progressionManager;
        ElementPickupObjectPooling elementPickupObjectPooling;
        Animation[] allAnimationComponents;

        float productionTimer = 0f;

        void Awake()
        {
            inventory = GetComponent<Inventory>();
            progressionManager = FindObjectOfType<ProgressionManager>();
            elementPickupObjectPooling = FindObjectOfType<ElementPickupObjectPooling>();
            allAnimationComponents = GetComponentsInChildren<Animation>();
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
            if (!CheckIfElementIsNeeded(elementToAdd) || !fullyPlaced) return;
            elementPickupObjectPooling.DestroyPickup(elementToAdd);

            inventory.EquipItemAt(elementToAdd.GetItemToEquip(), GetElementInstructionIndex(elementToAdd.GetItemToEquip().element));
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
            if (possibleInstructions[instructionIndex] != currentInstruction) inventory.ResetInventory(); //TODO: Look at options instead of removing all elements in machine
            currentInstruction = possibleInstructions[instructionIndex];
            UpdateElementTriggers();
            OnInstructionChanged();
        }
        public void SetCurrentInstruction(Instruction instruction)
        {
            currentInstruction = instruction;
            UpdateElementTriggers();
            OnInstructionChanged();
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
                elementPickupObjectPooling.InstantiatePickup(currentInstruction.outcome.element.pickupPrefab.GetComponent<ElementPickup>(), elementExitPoint.position + UnityEngine.Random.insideUnitSphere*.2f, Vector3.zero);   
            }
            inventory.RemoveItem(currentInstruction.requirements);

        }


        private void ToggleAnimations(bool isActive)
        {
            if (isActive == allAnimationComponents[0].isPlaying) return;
            foreach(Animation animation in allAnimationComponents)
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

        public void PlacementStatusChange(Builder player, PlacementStatus placementStatus)
        {
            switch (placementStatus)
            {
                case PlacementStatus.startingPlacement:
                    builder = player;
                    playerInventory = player.GetComponent<Inventory>();
                    GetComponent<NavMeshObstacle>().enabled = true;
                    GetComponent<BoxCollider>().enabled = true;
                    break;
                case PlacementStatus.endingPlacement:
                    fullyPlaced = true;
                    break;
                case PlacementStatus.startingDemolishment:
                    fullyPlaced = false;
                    break;
                case PlacementStatus.endingDemolishment:
                    Destroy(gameObject);
                    break;
            }
        }
        public bool IsFullyPlaced()
        {
            return fullyPlaced;
        }
        public void ChangeVersion()
        {
            SetCurrentInstruction((GetCurrentInstructionIndex() + 1) % possibleInstructions.Length);
        }
        public PlaceableInformation GetPlaceableInformation()
        {
            return placeableInformation;
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

        public CursorType GetCursorType()
        {
            return CursorType.Placeable;
        }

        public bool InRange(Transform player)
        {
            return fullyPlaced && !builder.IsDemolishMode();
        }

        public void HandleInteractionClick(Transform player)
        {
            if (EventSystem.current.IsPointerOverGameObject() || builder.IsDemolishMode()) return;

            OnMachineClicked();
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