using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Elements
{
    public class ElementTrigger : MonoBehaviour
    {
        [Tooltip("Can be null if no filter needed")][SerializeField] Element filter;
        [SerializeField] UnityEvent<ElementPickup> onElementEnterTrigger;
        [SerializeField] UnityEvent<ElementPickup> onElementStayTrigger;
        [SerializeField] UnityEvent<ElementPickup> onElementEnterCollider;
        [SerializeField] UnityEvent<ElementPickup> onElementStayCollider;

        public void UpdateFilter(Element newFilter)
        {
            filter = newFilter;
        }
        private void OnCollisionEnter(Collision other)
        {
            ElementPickup elementPickup = other.collider.GetComponentInParent<ElementPickup>();
            if (elementPickup != null && CheckFilter(elementPickup.GetItemToEquip().element))
            {
                onElementEnterCollider.Invoke(elementPickup);
            }
        }
        private void OnCollisionStay(Collision other)
        {
            ElementPickup elementPickup = other.collider.GetComponentInParent<ElementPickup>();
            if (elementPickup != null && CheckFilter(elementPickup.GetItemToEquip().element))
            {
                onElementStayCollider.Invoke(elementPickup);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null && CheckFilter(elementPickup.GetItemToEquip().element))
            {
                onElementEnterTrigger.Invoke(elementPickup);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null && CheckFilter(elementPickup.GetItemToEquip().element))
            {
                onElementStayTrigger.Invoke(elementPickup);
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