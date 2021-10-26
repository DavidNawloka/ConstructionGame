using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CON.Elements;

namespace CON.UI
{
    public class InventoryVisualisation : MonoBehaviour
    {
        [SerializeField] Image[] inventorySlotsSprites;
        [SerializeField] TextMeshProUGUI[] inventorySlotsAmounts;

        public bool isVisible = true;

        public void ToggleVisibility()
        {
            isVisible = !isVisible;
        }

        public Vector3 GetInventorySlotScreenPos(int slotIndex)
        {
            return inventorySlotsSprites[slotIndex].transform.position;
        }

        public void UpdateInventory(Inventory inventory)
        {
            InventoryItem[] updatedInventory = inventory.GetInventoryArray();

            for (int inventoryIndex = 0; inventoryIndex < updatedInventory.Length; inventoryIndex++)
            {
                Element element = updatedInventory[inventoryIndex].element;


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
