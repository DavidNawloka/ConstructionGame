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
        [SerializeField] Image[] requirementSprite;
        [SerializeField] TextMeshProUGUI[] requirementTMPro;
        [SerializeField] TextMeshProUGUI[] inventorySlotTMPro;
        [SerializeField] RectTransform sliderForeground;
        [SerializeField] RectTransform horizontalConnection;
        [SerializeField] RectTransform verticalConnection;
        [SerializeField] Image[] requirementImages;
        [SerializeField] Image outcomeImage;
        [SerializeField] TMP_Dropdown instructionDropdown;

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
            SetActiveCanvas(false);
            UpdateRequirementUIMultiple();
            UpdateRequirementUIOnce();
        }
        private void OnEnable()
        {
            machine.OnMachineClicked += MachineClicked;
        }
        private void OnDisable()
        {
            machine.OnMachineClicked -= MachineClicked;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
            player.GetComponent<Builder>().onBuildModeChange -= SetActiveElementIndicators;
        }
        private void Update()
        {
            if (isHidden) return;
            UpdateOwnPosition();
            UpdateConnectionPosition();
            sliderForeground.localScale = new Vector3(machine.GetProductionFraction(), 1, 1);
        }

        public void UpdateCurrentInstruction(int instructionIndex)
        {
            machine.SetCurrentInstruction(instructionIndex);
            UpdateRequirementUIMultiple();
        }

        public void UpdateInventorySlot(Inventory inventory) // Event from Inventory Class function from Machine
        {
            for (int requirementIndex = 0; requirementIndex < machine.GetCurrentInstruction().requirements.Length; requirementIndex++)
            {
                if (inventory.HasItem(machine.GetCurrentInstruction().requirements[requirementIndex]))
                {
                    inventorySlotTMPro[requirementIndex].color = Color.white;
                }
                else inventorySlotTMPro[requirementIndex].color = Color.red;
            }
        }

        public void AddEnergy() // Button Function
        {
            machine.AddAllEnergyElements();
        }

        public void MachineClicked() // Event from Machine Class function
        {
            transform.position = Camera.main.WorldToScreenPoint(machine.transform.position);
            if (isFirstClick) SetActiveCanvas(true);
            else SetActiveCanvas(false);
        }

        public void SetActiveCanvas(bool isActive) // Button function
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

        private void SetActiveElementIndicators(bool isActive)
        {
            foreach (Image requirementImage in requirementImages)
            {
                requirementImage.enabled = isActive;
            }
            outcomeImage.enabled = isActive;
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

            // Depends on size of game window, something is locally calculated
            horizontalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.x), horizontalConnection.sizeDelta.y);
            verticalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.y), verticalConnection.sizeDelta.y);

            horizontalConnection.position = new Vector3((posDifference.x / 2) + machineScreenSpacePosition.x, transform.position.y);

            verticalConnection.position = new Vector3(machineScreenSpacePosition.x, (posDifference.y / 2)+machineScreenSpacePosition.y);
        }
        private void UpdateRequirementUIMultiple()
        {
            Instruction machineInstruction = machine.GetCurrentInstruction();

            bool isNeeded = machineInstruction.requirements.Length >= 2;
            requirementTMPro[1].enabled = isNeeded;
            requirementSprite[1].enabled = isNeeded;

            isNeeded = machineInstruction.requirements.Length >= 3;
            requirementTMPro[2].enabled = isNeeded;
            requirementSprite[2].enabled = isNeeded;

            for (int inventoryIndex = 0; inventoryIndex < machineInstruction.requirements.Length; inventoryIndex++)
            {
                requirementSprite[inventoryIndex].sprite = machineInstruction.requirements[inventoryIndex].element.sprite;
                requirementTMPro[inventoryIndex].text = machineInstruction.requirements[inventoryIndex].amount.ToString();
            }
            for (int requirementIndex = 0; requirementIndex < machineInstruction.requirements.Length; requirementIndex++)
            {
                requirementImages[requirementIndex].sprite = machineInstruction.requirements[requirementIndex].element.sprite;
            }
            outcomeImage.sprite = machineInstruction.outcome.element.sprite;

        }

        private void UpdateRequirementUIOnce()
        {
            instructionDropdown.options.Clear();

            foreach (Instruction instruction in machine.GetPossibleInstructions())
            {
                instructionDropdown.options.Add(new TMP_Dropdown.OptionData() { text = instruction.outcome.element.name, image = instruction.outcome.element.sprite });
            }

            instructionDropdown.value = machine.GetCurrentInstructionIndex();

            if (!machine.GetFullyPlacedStatus()) SetActiveElementIndicators(true);

            GameObject.FindGameObjectWithTag("Player").GetComponent<Builder>().onBuildModeChange += SetActiveElementIndicators;
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
