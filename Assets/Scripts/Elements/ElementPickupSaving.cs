using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astutos.Saving;

namespace CON.Elements
{
    public class ElementPickupSaving : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform elementPickupParent;
        public object CaptureState()
        {
            ElementPickup[] elementPickups = FindObjectsOfType<ElementPickup>();

            SavedElementPickup[] savedElementPickups = new SavedElementPickup[elementPickups.Length];

            for (int index = 0; index < elementPickups.Length; index++)
            {
                ElementPickup currentElementPickup = elementPickups[index];
                savedElementPickups[index] = new SavedElementPickup(new InventoryItem(currentElementPickup.GetElementPickupType(),currentElementPickup.GetItemToEquip().amount), currentElementPickup.transform.position, currentElementPickup.transform.rotation,currentElementPickup.respawnable,currentElementPickup.isVisible);
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
                GameObject instantiatedPickup = Instantiate(currentElementPickup.elementToEquip.DeSerialize().element.pickupPrefab, currentElementPickup.position.ToVector(), Quaternion.Euler(currentElementPickup.rotation.ToVector()), elementPickupParent);
                instantiatedPickup.GetComponent<ElementPickup>().UpdateAmoutToEquip(currentElementPickup.elementToEquip.amount);
                instantiatedPickup.GetComponent<ElementPickup>().respawnable = currentElementPickup.isRespawnable;
                instantiatedPickup.GetComponent<ElementPickup>().isVisible = currentElementPickup.isVisible;
            }
        }

        [System.Serializable]
        private class SavedElementPickup
        {
            public SerializeableInventoryItem elementToEquip;
            public SerializableVector3 position;
            public SerializableVector3 rotation;
            public bool isRespawnable;
            public bool isVisible;

            public SavedElementPickup(InventoryItem elementToEquip, Vector3 position, Quaternion rotation, bool isRespawnable, bool isVisible)
            {
                this.elementToEquip = elementToEquip.Serialize();
                this.position = new SerializableVector3(position);
                this.rotation = new SerializableVector3(rotation.eulerAngles);
                this.isRespawnable = isRespawnable;
                this.isVisible = isVisible;
            }
        }

    }

}