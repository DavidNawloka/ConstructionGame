using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CON.Elements
{
    public class InventoryVisualisation : MonoBehaviour
    {
        [SerializeField] Image[] inventorySlotsSprites;
        [SerializeField] TextMeshProUGUI[] inventorySlotsAmounts;

        private void OnEnable()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().OnInventoryChange += UpdateInventory;
        }
        private void OnDisable()
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>().OnInventoryChange -= UpdateInventory;
        }
        private void UpdateInventory(InventoryItem[] updatedInventory)
        {
            for (int inventoryIndex = 0; inventoryIndex < updatedInventory.Length; inventoryIndex++)
            {
                Element element = updatedInventory[inventoryIndex].element; // TODO: Check if null reference error still exists and if so fix it


                if (element == null)
                {
                    inventorySlotsSprites[inventoryIndex].sprite = null;
                    inventorySlotsSprites[inventoryIndex].color = new Color(0,0,0,0);
                    inventorySlotsAmounts[inventoryIndex].text = "";
                    continue;
                }

                inventorySlotsSprites[inventoryIndex].color = new Color(1,1,1,1);
                inventorySlotsSprites[inventoryIndex].sprite = element.sprite;
                inventorySlotsAmounts[inventoryIndex].text = updatedInventory[inventoryIndex].amount.ToString();
            }
        }
    }
}
