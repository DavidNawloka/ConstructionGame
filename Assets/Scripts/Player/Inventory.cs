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

        private void BuildEmptyInventory()
        {
            inventory = new InventoryItem[inventorySlots];
            for (int index = 0; index < inventory.Length; index++)
            {
                inventory[index] = new InventoryItem(null, 0);
            }
        }

        private void Start()
        {
            OnInventoryChange(inventory);
        }

        public bool EquipItem(Element element, int amount)
        {
            if (GetUsedInventoryLength() >= inventorySlots) return false;

            int materialIndex = 0;

            foreach(InventoryItem item in inventory)
            {
                if(item.element == element)
                {
                    item.amount += amount;
                    OnInventoryChange(inventory);
                    return true;
                }
                if (item.element != null) materialIndex++;
            }

            inventory[materialIndex] = new InventoryItem(element, amount);
            OnInventoryChange(inventory);
            return true;
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