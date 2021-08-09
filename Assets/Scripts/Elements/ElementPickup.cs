using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour
    {
        [SerializeField] InventoryItem itemToEuqip;
        private void OnTriggerEnter(Collider other) // Consider having to click on it
        {
            other.transform.GetComponent<Inventory>().EquipItem(itemToEuqip);
            Destroy(gameObject);
        }
    }

}