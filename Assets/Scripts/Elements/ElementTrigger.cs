using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Elements
{
    public class ElementTrigger : MonoBehaviour
    {
        [SerializeField] UnityEvent<ElementPickup> onElementEnter;
        [SerializeField] UnityEvent<ElementPickup> onElementStay;
        private void OnTriggerEnter(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null)
            {
                onElementEnter.Invoke(elementPickup);
            }
        }
        private void OnTriggerStay(Collider other)
        {
            ElementPickup elementPickup = other.GetComponentInParent<ElementPickup>();
            if (elementPickup != null)
            {
                onElementStay.Invoke(elementPickup);
            }
        }
    }

}