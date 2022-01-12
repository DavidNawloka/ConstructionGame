using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astutos.Saving;

namespace CON.Elements
{
    public class ElementPickupSaving : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform elementPickupParent;

        ElementPickupObjectPooling elementPickupObjectPooling;

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
            elementPickupObjectPooling = FindObjectOfType<ElementPickupObjectPooling>();

            ElementPickup[] elementPickups = FindObjectsOfType<ElementPickup>();
            for (int index = 0; index < elementPickups.Length; index++)
            {
                Destroy(elementPickups[index].gameObject);
            }

            SavedElementPickup[] savedElementPickups = (SavedElementPickup[])state;


            for (int index = 0; index < savedElementPickups.Length; index++)
            {
                SavedElementPickup savedElementPickup = savedElementPickups[index];
                ElementPickup instantiatedElementPickup = Instantiate(savedElementPickup.elementToEquip.DeSerialize().element.pickupPrefab, savedElementPickup.position.ToVector(), Quaternion.Euler(savedElementPickup.rotation.ToVector()), elementPickupParent).GetComponent<ElementPickup>();
                instantiatedElementPickup.UpdateAmoutToEquip(savedElementPickup.elementToEquip.amount);
                instantiatedElementPickup.respawnable = savedElementPickup.isRespawnable;
                instantiatedElementPickup.isVisible = savedElementPickup.isVisible;
                if(!savedElementPickup.isRespawnable && !savedElementPickup.isVisible) 
                {
                    elementPickupObjectPooling.AddPickupToDictionary(instantiatedElementPickup);
                }
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