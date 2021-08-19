using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CON.Elements;
using UnityEngine.EventSystems;

namespace CON.Machines
{
    public class MachineVisualisation : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
    {
        [SerializeField] Image requirementSprite;
        [SerializeField] TextMeshProUGUI requirementTMPro;
        [SerializeField] RectTransform sliderForeground;

        RectTransform rectTransform;
        Transform mainCamera;
        CanvasGroup canvasGroup;
        Machine machine;

        bool isFirstClick;
        bool isHidden = true;

        bool followMouse = false;
        Vector3 initialMousePosition;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            mainCamera = GameObject.FindGameObjectWithTag("FollowCamera").transform;
        }
        private void Start()
        {
            machine.OnMachineClicked.AddListener(MachineClicked);
            SetCanvasGroup(false);
            UpdateRequirementUI();
        }

        private void Update()
        {
            if (isHidden) return;

            if (followMouse)
            {
                transform.position = Input.mousePosition + initialMousePosition;
            }

            sliderForeground.localScale = new Vector3(machine.GetProductionFraction(), 1, 1);
        }

        public void AddEnergy()
        {
            machine.AddEnergyElement();
        }
        public void SetMachine(Machine machine)
        {
            this.machine = machine;
        }

        public void MachineClicked()
        {
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
