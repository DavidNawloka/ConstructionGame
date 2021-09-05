using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CON.Elements;
using UnityEngine.EventSystems;
using System;
using CON.Machines;

namespace CON.UI
{
    public class MachineVisualisation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Image requirementSprite;
        [SerializeField] TextMeshProUGUI requirementTMPro;
        [SerializeField] TextMeshProUGUI inventorySlotTMPro;
        [SerializeField] RectTransform sliderForeground;
        [SerializeField] RectTransform horizontalConnection;
        [SerializeField] RectTransform verticalConnection;

        CanvasGroup canvasGroup;
        Machine machine;

        bool isFirstClick;
        bool isHidden = true;

        bool followMouse = false;
        Vector3 initialMousePosition;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            machine = GetComponentInParent<Machine>();
        }
        private void Start()
        {
            SetCanvasGroup(false);
            UpdateRequirementUI();
        }

        private void Update()
        {
            if (isHidden) return;
            UpdateOwnPosition();
            UpdateConnectionPosition();
            sliderForeground.localScale = new Vector3(machine.GetProductionFraction(), 1, 1);
        }

        public void UpdateInventorySlot(Inventory inventory)
        {
            if (inventory.HasItem(machine.GetEnergyRequirement()))
            {
                inventorySlotTMPro.color = Color.white;
            }
            else inventorySlotTMPro.color = Color.red;
        }

        public void AddEnergy() // Button Function
        {
            machine.AddAllEnergyElements();
        }

        public void MachineClicked() // Event from Machine Class Function
        {
            transform.position = Camera.main.WorldToScreenPoint(machine.transform.position);
            if (isFirstClick) SetCanvasGroup(true);
            else SetCanvasGroup(false);
        }

        public void SetCanvasGroup(bool isActive) // Button Function
        {
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
            horizontalConnection.gameObject.SetActive(isActive);
            verticalConnection.gameObject.SetActive(isActive);

            isFirstClick = !isActive;
            isHidden = !isActive;

            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;
        }
        private void UpdateOwnPosition()
        {
            if (followMouse)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(Input.mousePosition.x + initialMousePosition.x, 0, Screen.currentResolution.width),
                    Mathf.Clamp(Input.mousePosition.y + initialMousePosition.y, 0, Screen.currentResolution.height),
                    0);
            }
        }
        private void UpdateConnectionPosition()
        {
            Vector3 machineScreenSpacePosition = Camera.main.WorldToScreenPoint(machine.transform.position);
            Vector3 posDifference = transform.position - machineScreenSpacePosition;

            horizontalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.x)*1.2f, horizontalConnection.sizeDelta.y);
            verticalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.y) * 1.2f, verticalConnection.sizeDelta.y);

            horizontalConnection.position = new Vector3((posDifference.x / 2) + machineScreenSpacePosition.x, transform.position.y);

            verticalConnection.position = new Vector3(machineScreenSpacePosition.x, (posDifference.y / 2)+machineScreenSpacePosition.y);
        }
        private void UpdateRequirementUI()
        {
            InventoryItem energyRequirement = machine.GetEnergyRequirement()[0];
            requirementSprite.sprite = energyRequirement.element.sprite;
            requirementTMPro.text = energyRequirement.amount.ToString();
        }

        // Interface Implementations
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                followMouse = true;
                initialMousePosition = transform.position-Input.mousePosition;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                followMouse = false;
            }
        }
    }
}
