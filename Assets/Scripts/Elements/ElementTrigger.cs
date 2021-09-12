using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Elements
{
    public class ElementTrigger : MonoBehaviour
    {
        [Tooltip("Can be null if no filter needed")][SerializeField] Element filter;
        [SerializeField] bool shouldUpdateFilterPerInstruction = true;
        [SerializeField] UnityEvent<ElementPickup> onElementEnter;
        [SerializeField] UnityEvent<ElementPickup> onElementStay;

        public void UpdateFilter(Element newFilter)
        {
            filter = newFilter;
        }

        private void OnTriggerEnter(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null && CheckFilter(elementPickup.GetItemToEquip().element))
            {
                onElementEnter.Invoke(elementPickup);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null && CheckFilter(elementPickup.GetItemToEquip().element))
            {
                onElementStay.Invoke(elementPickup);
            }
        }

        private bool CheckFilter(Element elementToCheck)
        {
            if (filter == null) return true;
            else if (filter == elementToCheck) return true;
            
            return false;
        }
    }

}