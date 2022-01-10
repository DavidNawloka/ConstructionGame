using CON.Core;
using CON.Player;
using CON.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CON.Elements
{

    public class ElementPickup : MonoBehaviour,IRaycastable
    {
        [SerializeField] Element elementPickupType;
        [SerializeField] InventoryItem itemToEuqip;
        [SerializeField] float maxDistance = 2f;
        [SerializeField] float maxElementMove = 2f;
        [SerializeField] float spawnedElementImageScale = 0.7f;
        [SerializeField] float spawnedElementImageAlpha = 0.7f;
        [SerializeField] AudioClip[] pickupSounds;
        [SerializeField] float pitchRange;
        public bool respawnable;

        float timer = 0f;

        [HideInInspector] public bool isVisible = true;

        AudioSource audioSource;
        MeshRenderer meshRenderer;
        MeshCollider meshCollider;
        Rigidbody rigidBody;


        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshCollider = GetComponentInChildren<MeshCollider>();
            rigidBody = GetComponent<Rigidbody>();
        }
        private void Start()
        {
            if(!isVisible && ShouldRespawn())
            {
                isVisible = true;
                UpdateVisibility();
            }
        }
        public Element GetElementPickupType()
        {
            return elementPickupType;
        }

        public InventoryItem GetItemToEquip()
        {
            return itemToEuqip;
        }
        public void UpdateAmoutToEquip(int amount)
        {
            itemToEuqip.amount = amount;
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
                PlayEquipSound(randIndex);

                isVisible = false;
                UpdateVisibility();

                yield return StartCoroutine(InitialiseElementMovement(inventoryIndex));
                yield return new WaitForSeconds(pickupSounds[randIndex].length - timer);
                playerInventory.EquipItem(itemToEuqip);
            }
        }


        private void UpdateVisibility()
        {
            if (isVisible) meshRenderer.renderingLayerMask = 2;
            else meshRenderer.renderingLayerMask = 0;

            meshCollider.enabled = isVisible;
            rigidBody.isKinematic = !isVisible;
        }

        private bool ShouldRespawn()
        {
            return !meshRenderer.isVisible;
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


        

        private void PlayEquipSound(int randIndex)
        {
            ChangePitch();
            audioSource.PlayOneShot(pickupSounds[randIndex]);
        }
        private void ChangePitch()
        {
            audioSource.pitch = Random.Range(1 - pitchRange / 2, 1 + pitchRange / 2);
        }
        // Interface implementations

        public void HandleInteractionClick(Transform player)
        {
            StartCoroutine(EquipElement(player));
        }
        public bool InRange(Transform player)
        {
            if (Vector3.Distance(transform.position, player.position) <= maxDistance)
            {
                return true;
            }
            return false;
        }
        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }

}