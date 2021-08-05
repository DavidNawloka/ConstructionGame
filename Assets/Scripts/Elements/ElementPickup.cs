using CON.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour
    {
        [SerializeField] Element element;
        [SerializeField] int amount = 1;
        private void OnTriggerEnter(Collider other) // Consider having to click on it
        {
            other.transform.GetComponent<Inventory>().EquipItem(element, amount);
            Destroy(gameObject);
        }
    }

}