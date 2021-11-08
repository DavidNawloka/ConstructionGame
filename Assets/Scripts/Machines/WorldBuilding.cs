using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CON.UI;
using Astutos.Saving;
using CON.Core;

namespace CON.Machines
{
    public class WorldBuilding : MonoBehaviour, ISaveable, IRaycastable
    {
        [SerializeField] UnlockRequirement[] unlockRequirements;
        [SerializeField] MoveableWindow moveableWindow;
        [SerializeField] Transform moveableWindowConnect;
        [SerializeField] GameObject brokenWorldBuilding;
        [SerializeField] GameObject fixedWorldBuilding;

        Inventory inventory;
        Builder builder;
        bool isFixed = false;

        void Awake()
        {
            inventory = GetComponent<Inventory>();
            builder = GameObject.FindGameObjectWithTag("Player").GetComponent<Builder>();
            builder.onBuildModeChange += OnBuildModeChange;
        }

        private void Start()
        {
            InitialiseElementTriggers();
        }
        
        public void AddEnergyElement(ElementPickup elementToAdd) // Event for when Elements enter
        {
            if (!CheckIfElementIsNeeded(elementToAdd)) return;
            Destroy(elementToAdd.gameObject);

            inventory.EquipItemAt(elementToAdd.GetItemToEquip(), GetElementInstructionIndex(elementToAdd.GetItemToEquip().element));

            if(!isFixed) CheckIfAllItems();
        }

        public UnlockRequirement[] GetUnlockRequirements()
        {
            return unlockRequirements;
        }

        private void CheckIfAllItems()
        {
            foreach (UnlockRequirement unlockRequirement in unlockRequirements)
            {
                if (!inventory.HasItem(unlockRequirement.inventoryItem)) return;
            }
            ConstructWorldBuilding();
        }

        private void ConstructWorldBuilding()
        {
            isFixed = true;
            brokenWorldBuilding.SetActive(false);
            fixedWorldBuilding.SetActive(true);
        }
        private int GetElementInstructionIndex(Element element)
        {
            for (int index = 0; index < unlockRequirements.Length; index++)
            {
                if (unlockRequirements[index].inventoryItem.element == element) return index;
            }
            return -1;
        }
        private bool CheckIfElementIsNeeded(ElementPickup elementToAdd)
        {
            foreach (UnlockRequirement unlockRequirement in unlockRequirements)
            {
                if (elementToAdd.GetItemToEquip().element == unlockRequirement.inventoryItem.element) return true;
            }
            return false;
        }
        private void InitialiseElementTriggers()
        {
            foreach(UnlockRequirement unlockRequirement in unlockRequirements)
            {
                unlockRequirement.elementTrigger.UpdateFilter(unlockRequirement.inventoryItem.element);
                unlockRequirement.requirementImage.sprite = unlockRequirement.inventoryItem.element.sprite;
                unlockRequirement.slotImage.sprite = unlockRequirement.inventoryItem.element.sprite;
                unlockRequirement.amountTMPro.text = unlockRequirement.inventoryItem.amount.ToString();
            }
        }
        private void OnBuildModeChange(bool isActive)
        {
            foreach (UnlockRequirement unlockRequirement in unlockRequirements)
            {
                unlockRequirement.requirementImage.gameObject.SetActive(isActive);
            }
        }
        public CursorType GetCursorType()
        {
            return CursorType.Placeable;
        }

        public bool InRange(Transform player)
        {
            return true;
        }

        public void HandleInteractionClick(Transform player)
        {
            moveableWindow.ToggleCanvas(moveableWindowConnect);
        }
        public object CaptureState()
        {
            return isFixed;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;
            isFixed = (bool)state;
            if (isFixed) ConstructWorldBuilding();
        }
    }

    [System.Serializable]
    public class UnlockRequirement
    {
        public ElementTrigger elementTrigger;
        public InventoryItem inventoryItem;
        public Image requirementImage;
        public Image slotImage;
        public TextMeshProUGUI amountTMPro;
    }

}