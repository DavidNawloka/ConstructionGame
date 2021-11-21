using Astutos.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Elements
{
    public class Inventory : MonoBehaviour, ISaveable
    {
        [SerializeField] int inventorySlots;

        private InventoryItem[] inventory;

        public UnityEvent<Inventory> OnInventoryChange;

        private void Awake()
        {
            if (inventory == null) BuildEmptyInventory();
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
                if (inventoryItem.element != inventoryItemToRemove.element) continue;

                if (inventoryItem.amount == inventoryItemToRemove.amount)
                {
                    inventoryItem.amount -= inventoryItemToRemove.amount;
                    inventoryItem.element = null;
                    status = true;
                }
                else if (inventoryItem.amount > inventoryItemToRemove.amount)
                {
                    inventoryItem.amount -= inventoryItemToRemove.amount;
                    status = true;
                }
                else
                {
                    Debug.LogError("Amount to be removed of " + inventoryItemToRemove.element + " is "
                        + inventoryItemToRemove.amount + " , existing amount available " + inventoryItem.amount + " !");
                }
            }
            OnInventoryChange.Invoke(this);
            return status;
        }
        public bool RemoveItem(InventoryItem[] inventoryItemsToRemove)
        {
            foreach (InventoryItem inventoryItem in inventoryItemsToRemove)
            {
                if(!RemoveItem(inventoryItem)) return false;
            }
            return true;
        }
        public bool EquipItemWhere(InventoryItem inventoryItemToEquip, out int inventoryPosIndex)
        {
            for (int inventoryIndex = 0; inventoryIndex < inventory.Length; inventoryIndex++)
            {
                if (inventory[inventoryIndex].element == inventoryItemToEquip.element || inventory[inventoryIndex].element == null)
                {
                    inventoryPosIndex = inventoryIndex;
                    return true;
                }
            }

            inventoryPosIndex = -1;
            return false;
        }
        public bool EquipItem(InventoryItem inventoryItemToEquip)
        {
            int indexToEquip = -1;
            for (int inventoryIndex = 0; inventoryIndex < inventory.Length; inventoryIndex++)
            {
                if (inventory[inventoryIndex].element == inventoryItemToEquip.element)
                {
                    inventory[inventoryIndex].amount += inventoryItemToEquip.amount;
                    OnInventoryChange.Invoke(this);
                    return true;
                }
                if(inventory[inventoryIndex].element == null)
                {
                    if(indexToEquip == -1) indexToEquip = inventoryIndex;
                }
            }

            if(indexToEquip != -1)
            {
                inventory[indexToEquip].amount = inventoryItemToEquip.amount;
                inventory[indexToEquip].element = inventoryItemToEquip.element;
                OnInventoryChange.Invoke(this);
                return true;
            }

            return false;
        }
        public bool EquipItem(InventoryItem[] inventoryItemsToEquip)
        {
            foreach (InventoryItem inventoryItem in inventoryItemsToEquip)
            {
                if(!EquipItem(inventoryItem)) return false;
            }
            return true;
        }
        public bool EquipItemAt(InventoryItem inventoryItemToEquip, int inventorySlotIndex)
        {
            if (inventory[inventorySlotIndex].element == inventoryItemToEquip.element)
            {
                inventory[inventorySlotIndex].amount += inventoryItemToEquip.amount;
                OnInventoryChange.Invoke(this);
                return true;
            }
            if (inventory[inventorySlotIndex].element == null)
            {
                inventory[inventorySlotIndex].element = inventoryItemToEquip.element;
                inventory[inventorySlotIndex].amount = inventoryItemToEquip.amount;
                OnInventoryChange.Invoke(this);
                return true;
            }
            else return false;
            
        }
        public int GetAmountOfElement(Element element)
        {
            foreach (InventoryItem inventoryItem in inventory)
            {
                if(inventoryItem.element == element)
                {
                    return inventoryItem.amount;
                }
            }
            return 0;
        }
        private void BuildEmptyInventory()
        {
            inventory = new InventoryItem[inventorySlots];
            for (int index = 0; index < inventory.Length; index++)
            {
                inventory[index] = new InventoryItem(null, 0);
            }
        }

        public object CaptureState()
        {
            SerializeableInventoryItem[] savedInventory = new SerializeableInventoryItem[inventory.Length];

            for (int index = 0; index < inventory.Length; index++)
            {
                InventoryItem currentInventoryItem = inventory[index];

                if (currentInventoryItem.element == null) savedInventory[index] = null;
                else
                {
                    savedInventory[index] = currentInventoryItem.Serialize();
                }
            }
            return savedInventory;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;

            SerializeableInventoryItem[] savedInventory = (SerializeableInventoryItem[])state;

            inventory = new InventoryItem[savedInventory.Length];

            for (int index = 0; index < inventory.Length; index++)
            {
                SerializeableInventoryItem currentInventoryItem = savedInventory[index];

                if (currentInventoryItem == null) inventory[index] = new InventoryItem(null, 0);
                else
                {
                    inventory[index] = currentInventoryItem.DeSerialize();
                }
            }
            OnInventoryChange.Invoke(this);
        }

        
    }
    [System.Serializable]
    public class SerializeableInventoryItem
    {
        public string elementName;
        public int amount;

        public SerializeableInventoryItem(string elementName, int amount)
        {
            this.elementName = elementName;
            this.amount = amount;
        }

        public InventoryItem DeSerialize()
        {
            return new InventoryItem((Element)Resources.Load(elementName),amount);
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

        public SerializeableInventoryItem Serialize()
        {
            return new SerializeableInventoryItem(element.name, amount);
        }
    }

}