using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{
    public class ElementPickupObjectPooling : MonoBehaviour
    {
        [SerializeField] Element[] allAvailableElements;
        [SerializeField] Dictionary<Element,Queue<ElementPickup>> instantiatedPickups = new Dictionary<Element, Queue<ElementPickup>>();

        private void Awake()
        {
            foreach(Element element in allAvailableElements)
            {
                instantiatedPickups.Add(element, new Queue<ElementPickup>());
            }
        }
#if UNITY_EDITOR
        private void Update()
        {
            if (!Debug.isDebugBuild) return;
            if (Input.GetKeyDown(KeyCode.F6))
            {
                foreach (KeyValuePair<Element, Queue<ElementPickup>> keyValuePair in instantiatedPickups)
                {
                    print(keyValuePair.Key.name + ": " + keyValuePair.Value.Count);
                }
            }
        }
#endif
        public void InstantiatePickup(ElementPickup elementPickup, Vector3 position, Vector3 rotation)
        {
            if(instantiatedPickups[elementPickup.GetItemToEquip().element].Count == 0)
            {
                print("Instantiated new Pickup: " + elementPickup.gameObject.name);
                Instantiate(elementPickup, position, Quaternion.Euler(rotation));
            }
            else
            {
                print("Reused old Pickup: " + elementPickup.gameObject.name);
                ElementPickup reusedPickup = instantiatedPickups[elementPickup.GetItemToEquip().element].Dequeue();
                reusedPickup.transform.position = position;
                reusedPickup.transform.rotation = Quaternion.Euler(rotation);
                reusedPickup.SetVisibilityActive(true);
            }
        }
        public void DestroyPickup(ElementPickup elementPickup)
        {
            //print("Destroyed Pickup: " + elementPickup.gameObject.name);
            elementPickup.SetVisibilityActive(false);
            elementPickup.transform.position = Vector3.zero;
            AddPickupToDictionary(elementPickup);
        }
        public void AddPickupToDictionary(ElementPickup elementPickup)
        {
            instantiatedPickups[elementPickup.GetItemToEquip().element].Enqueue(elementPickup);
        }
    }
}