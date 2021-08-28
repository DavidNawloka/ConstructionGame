using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Elements
{
    public class ElementTrigger : MonoBehaviour
    {
        [SerializeField] UnityEvent<InventoryItem> onElementEnter;
        private void OnTriggerEnter(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null)
            {
                onElementEnter.Invoke(elementPickup.GetItemToEquip());
                Destroy(elementPickup.gameObject);
            }
        }
    }

}