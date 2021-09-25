using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astutos.Saving;

namespace CON.Elements
{
    public class ElementSaving : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform elementPickupParent;
        public object CaptureState()
        {
            ElementPickup[] elementPickups = FindObjectsOfType<ElementPickup>();

            SavedElementPickup[] savedElementPickups = new SavedElementPickup[elementPickups.Length];

            for (int index = 0; index < elementPickups.Length; index++)
            {
                ElementPickup currentElementPickup = elementPickups[index];
                savedElementPickups[index] = new SavedElementPickup(currentElementPickup.GetItemToEquip(), currentElementPickup.transform.position, currentElementPickup.transform.rotation);
            }

            return savedElementPickups;
        }

        public void RestoreState(object state)
        {
            ElementPickup[] elementPickups = FindObjectsOfType<ElementPickup>();
            for (int index = 0; index < elementPickups.Length; index++)
            {
                Destroy(elementPickups[index].gameObject);
            }

            SavedElementPickup[] savedElementPickups = (SavedElementPickup[])state;


            for (int index = 0; index < savedElementPickups.Length; index++)
            {
                SavedElementPickup currentElementPickup = savedElementPickups[index];
                Instantiate(currentElementPickup.elementToEquip.DeSerialize().element.pickupPrefab, currentElementPickup.position.ToVector(), Quaternion.Euler(currentElementPickup.rotation.ToVector()), elementPickupParent);
            }
        }

        [System.Serializable]
        private class SavedElementPickup
        {
            public SerializeableInventoryItem elementToEquip;
            public SerializableVector3 position;
            public SerializableVector3 rotation;

            public SavedElementPickup(InventoryItem elementToEquip, Vector3 position, Quaternion rotation)
            {
                this.elementToEquip = elementToEquip.Serialize();
                this.position = new SerializableVector3(position);
                this.rotation = new SerializableVector3(rotation.eulerAngles);
            }
        }

    }

}