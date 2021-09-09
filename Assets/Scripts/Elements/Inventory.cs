using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Elements
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] int inventorySlots;

        public InventoryItem[] inventory;

        public UnityEvent<Inventory> OnInventoryChange;

        private void Awake()
        {
            BuildEmptyInventory();
        }
        private void Start()
        {
            OnInventoryChange.Invoke(this);
        }
        public InventoryItem[] GetInventoryArray()
        {
            return inventory;
        }
        public bool HasItem(InventoryItem inventoryItemToCheck)
        {
            foreach (InventoryItem inventoryItem in inventory)
            {
                if (inventoryItem.element == inventoryItemToCheck.element && inventoryItem.amount >= inventoryItemToCheck.amount)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasItem(InventoryItem[] inventoryItemsToCheck)
        {
            foreach(InventoryItem inventoryItem in inventoryItemsToCheck)
            {
                if (!HasItem(inventoryItem)) return false;
            }
            return true;
        }
        public bool RemoveItem(InventoryItem inventoryItemToRemove)
        {
            bool status = false;
            for (int index = 0; index < inventory.Length; index++)
            {
                InventoryItem inventoryItem = inventory[index];
                if (inventoryItem.element == inventoryItemToRemove.element)
                {
                    if(inventoryItem.amount == inventoryItemToRemove.amount)
                    {
                        inventoryItem = new InventoryItem(null, 0);
                        status = true;
                    }
                    else if(inventoryItem.amount > inventoryItemToRemove.amount)
                    {
                        inventoryItem.amount -= inventoryItemToRemove.amount;
                        status = true;
                    }
                    else
                    {
                        Debug.LogError("Amount to be removed of " + inventoryItemToRemove.element + " is " 
                            + inventoryItemToRemove.amount + " , existing amount available " + inventoryItem.amount + " !");
                    }
                    inventory[index] = inventoryItem;
                }
            }
            OnInventoryChange.Invoke(this);
            return status;
        }
        public bool RemoveItem(InventoryItem[] inventoryItemsToRemove)
        {
            foreach (InventoryItem inventoryItem in inventoryItemsToRemove)
            {
                RemoveItem(inventoryItem);
            }
            return true;
        }
        public bool EquipItem(InventoryItem inventoryItemToEquip)
        {
            int materialIndex = -1;

            for (int inventoryIndex = 0; inventoryIndex < inventory.Length; inventoryIndex++)
            {
                if (inventory[inventoryIndex].element == inventoryItemToEquip.element)
                {
                    inventory[inventoryIndex].amount += inventoryItemToEquip.amount;
                    OnInventoryChange.Invoke(this);
                    return true;
                }
                if(inventory[inventoryIndex].element == null && materialIndex == -1)
                {
                    materialIndex = inventoryIndex;
                }
            }

            if (GetUsedInventoryLength() >= inventorySlots) return false;

            inventory[materialIndex] = new InventoryItem(inventoryItemToEquip.element, inventoryItemToEquip.amount);
            OnInventoryChange.Invoke(this);
            return true;
        }
        public bool EquipItem(InventoryItem[] inventoryItemsToEquip)
        {
            foreach (InventoryItem inventoryItem in inventoryItemsToEquip)
            {
                EquipItem(inventoryItem);
            }
            return true;
        }
        private void BuildEmptyInventory()
        {
            inventory = new InventoryItem[inventorySlots];
            for (int index = 0; index < inventory.Length; index++)
            {
                inventory[index] = new InventoryItem(null, 0);
            }
        }
        private int GetUsedInventoryLength()
        {
            int length = 0;
            foreach (InventoryItem item in inventory)
            {
                if (item.element != null) length++;
            }
            return length;
        }

        
    }
    [System.Serializable]
    public class InventoryItem
    {
        public Element element;
        public int amount;

        public InventoryItem(Element element, int amount)
        {
            this.element = element;
            this.amount = amount;
        }
    }

}