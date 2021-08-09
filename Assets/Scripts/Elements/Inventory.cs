using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] int inventorySlots;

        public InventoryItem[] inventory;
        public event Action<InventoryItem[]> OnInventoryChange;

        private void Awake()
        {
            BuildEmptyInventory();
        }
        private void Start()
        {
            OnInventoryChange(inventory);
        }
        public bool CheckItem(InventoryItem inventoryItemToCheck)
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
            OnInventoryChange(inventory);
            return status;
        }
        public bool EquipItem(InventoryItem inventoryItemToEquip)
        {
            if (GetUsedInventoryLength() >= inventorySlots) return false;

            int materialIndex = 0;

            foreach(InventoryItem inventoryItem in inventory)
            {
                if(inventoryItem.element == inventoryItemToEquip.element)
                {
                    inventoryItem.amount += inventoryItemToEquip.amount;
                    OnInventoryChange(inventory);
                    return true;
                }
                if (inventoryItem.element != null) materialIndex++;
            }

            inventory[materialIndex] = new InventoryItem(inventoryItemToEquip.element, inventoryItemToEquip.amount);
            OnInventoryChange(inventory);
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