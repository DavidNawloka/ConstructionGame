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


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag != "Player") return;
            EquipElement(collision.transform);
        }
        private void EquipElement(Transform player)
        {
            player.GetComponent<Inventory>().EquipItem(itemToEuqip);
            Destroy(gameObject);
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