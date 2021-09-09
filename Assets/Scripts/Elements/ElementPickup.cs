using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour,IMouseClickable
    {
        [SerializeField] InventoryItem itemToEuqip;
        [SerializeField] float maxDistance = 2f;

        public InventoryItem GetItemToEquip()
        {
            return itemToEuqip;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Water") Destroy(gameObject);
            if (other.transform.tag != "Player") return;
            EquipElement(other.transform);
        }
        private void EquipElement(Transform player)
        {
            if (player.GetComponent<Inventory>().EquipItem(itemToEuqip))
            {
                Destroy(gameObject);
            }
        }

        // Interface implementations

        public bool HandleInteractionClick(Transform player)
        {
            if(Vector3.Distance(transform.position, player.position) <= maxDistance)
            {
                EquipElement(player);
                return true;
            }
            return false;
        }
        
    }

}