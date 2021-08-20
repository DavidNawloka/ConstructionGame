using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CON.Elements;
using UnityEngine.EventSystems;
using System;

namespace CON.Machines
{
    public class MachineVisualisation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Image requirementSprite;
        [SerializeField] TextMeshProUGUI requirementTMPro;
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

            //horizontalConnection.position = Camera.main.WorldToScreenPoint(machine.transform.position);

            horizontalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.x)*1.2f, horizontalConnection.sizeDelta.y);
            verticalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.y) * 1.2f, verticalConnection.sizeDelta.y);

            horizontalConnection.position = new Vector3((posDifference.x / 2) + machineScreenSpacePosition.x, transform.position.y);

            verticalConnection.position = new Vector3(machineScreenSpacePosition.x, (posDifference.y / 2)+machineScreenSpacePosition.y);
        }

        public void AddEnergy()
        {
            machine.AddEnergyElement();
        }

        public void MachineClicked()
        {
            transform.position = Camera.main.WorldToScreenPoint(machine.transform.position);
            if (isFirstClick) SetCanvasGroup(true);
            else SetCanvasGroup(false);
        }
        private void UpdateRequirementUI()
        {
            InventoryItem energyRequirement = machine.GetEnergyRequirement();
            requirementSprite.sprite = energyRequirement.element.sprite;
            requirementTMPro.text = energyRequirement.amount.ToString();
        }

        public void SetCanvasGroup(bool isActive)
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
