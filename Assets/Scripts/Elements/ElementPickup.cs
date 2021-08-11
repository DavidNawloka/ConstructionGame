using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour
    {
        [SerializeField] InventoryItem itemToEuqip;
        private void OnCollisionEnter(Collision collision) // Consider having to click on it
        {
            if (collision.transform.tag != "Player") return;
            collision.transform.GetComponent<Inventory>().EquipItem(itemToEuqip);
            Destroy(gameObject);
        }
    }

}