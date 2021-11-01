using CON.Core;
using CON.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour,IMouseClickable
    {
        [SerializeField] InventoryItem itemToEuqip;
        [SerializeField] float maxDistance = 2f;
        [SerializeField] float maxElementMove = 2f;
        [SerializeField] float spawnedElementImageScale = 0.7f;
        [SerializeField] float spawnedElementImageAlpha = 0.7f;
        [SerializeField] AudioClip[] pickupSounds;

        float timer = 0f;

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
            int inventoryIndex;
            Inventory playerInventory = player.GetComponent<Inventory>();
            if (playerInventory.EquipItemWhere(itemToEuqip, out inventoryIndex))
            {
                int randIndex = Random.Range(0, pickupSounds.Length);
                GetComponent<AudioSourceManager>().PlayOnce(pickupSounds[randIndex]);
                GetComponentInChildren<MeshRenderer>().enabled = false;
                GetComponentInChildren<MeshCollider>().enabled = false;

                yield return StartCoroutine(InitialiseElementMovement(inventoryIndex));
                yield return new WaitForSeconds(pickupSounds[randIndex].length - timer);
                playerInventory.EquipItem(itemToEuqip);
                Destroy(gameObject);
            }
        }

        private IEnumerator InitialiseElementMovement(int slotIndex)
        {
            InventoryVisualisation playerInventoryVisualisation = GameObject.FindGameObjectWithTag("PlayerInventoryVisualisation").GetComponent<InventoryVisualisation>();

            if (!playerInventoryVisualisation.isVisible) yield break;

            Vector3 targetPos = playerInventoryVisualisation.GetInventorySlotScreenPos(slotIndex);
            Transform mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;

            GameObject elementGameObject = new GameObject();
            elementGameObject.transform.parent = mainCanvas;
            elementGameObject.transform.position = Camera.main.WorldToScreenPoint(transform.position);

            Image elementImage = elementGameObject.AddComponent<Image>();
            elementImage.sprite = itemToEuqip.element.sprite;
            elementImage.color = new Color(1,1,1,spawnedElementImageAlpha);
            elementImage.transform.localScale = new Vector3(spawnedElementImageScale, spawnedElementImageScale, spawnedElementImageScale);

            yield return StartCoroutine(MoveElementSprite(elementGameObject,targetPos));

            Destroy(elementGameObject);

        }

        private IEnumerator MoveElementSprite(GameObject element,Vector3 targetPos)
        {
            while(Vector3.Distance(element.transform.position,targetPos) > 3f)
            {
                element.transform.position = Vector3.MoveTowards(element.transform.position, targetPos, Time.deltaTime * maxElementMove);
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(element);
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