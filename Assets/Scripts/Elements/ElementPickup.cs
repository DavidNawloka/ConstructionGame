using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour,IMouseClickable // TODO: Make Parent Element Pickup prefab, make all other prefab variants
    {
        [SerializeField] InventoryItem itemToEuqip;
        [SerializeField] float maxDistance = 2f;
        [SerializeField] AudioClip[] pickupSounds;

        public InventoryItem GetItemToEquip()
        {
            return itemToEuqip;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Water") Destroy(gameObject);
            if (other.transform.tag != "Player") return;
            StartCoroutine(EquipElement(other.transform));
        }
        private IEnumerator EquipElement(Transform player)
        {
            if (player.GetComponent<Inventory>().EquipItem(itemToEuqip))
            {
                int randIndex = Random.Range(0, pickupSounds.Length);
                GetComponent<AudioSourceManager>().PlayOnce(pickupSounds[randIndex]);
                GetComponentInChildren<MeshRenderer>().enabled = false;
                GetComponentInChildren<MeshCollider>().enabled = false;
                yield return new WaitForSeconds(pickupSounds[randIndex].length);
                Destroy(gameObject);
            }
        }

        // Interface implementations

        public bool HandleInteractionClick(Transform player)
        {
            if(Vector3.Distance(transform.position, player.position) <= maxDistance)
            {
                StartCoroutine(EquipElement(player));
                return true;
            }
            return false;
        }
        
    }

}