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
        [Header("Inventory")]
        [SerializeField] Image[] requirementSprite;
        [SerializeField] TextMeshProUGUI[] requirementTMPro;
        [SerializeField] TextMeshProUGUI[] inventorySlotTMPro;
        [SerializeField] RectTransform sliderForeground;
        [Header("Connectors")]
        [SerializeField] RectTransform horizontalConnection;
        [SerializeField] RectTransform verticalConnection;
        [Header("Machine specific")]
        [SerializeField] Image[] requirementImages;
        [SerializeField] Image outcomeImage;
        [Header("Instruction related")]
        [SerializeField] TMP_Dropdown instructionDropdown;
        [SerializeField] Image[] instructionIndicatorSprites;
        [SerializeField] TextMeshProUGUI[] instructionIndicatorTMPro;
        [SerializeField] Image instructionIndicatorOutcomeSprite;
        [SerializeField] TextMeshProUGUI instructionIndicatorOutcomeTMPro;

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
            UpdateInstructionIndicator();
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

        public void SetPauseActive(bool isPaused)
        {
            machine.SetPause(isPaused);
        }

        public void UpdateCurrentInstruction(int instructionIndex)
        {
            machine.SetCurrentInstruction(instructionIndex);
            UpdateRequirementUIMultiple();
            UpdateInstructionIndicator();
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


            horizontalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.x), horizontalConnection.sizeDelta.y);
            verticalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.y), verticalConnection.sizeDelta.y);

            horizontalConnection.position = new Vector3((posDifference.x / 2) + machineScreenSpacePosition.x, transform.position.y);

            verticalConnection.position = new Vector3(machineScreenSpacePosition.x, (posDifference.y / 2)+machineScreenSpacePosition.y);
        }
        private void UpdateInstructionIndicator()
        {
            Instruction machineInstruction = machine.GetCurrentInstruction();

            for (int index = 0; index < 3; index++)
            {
                instructionIndicatorSprites[index].sprite = null;
                instructionIndicatorSprites[index].color = new Color(0,0,0,0);
                instructionIndicatorTMPro[index].text = "";
            }

            switch (machineInstruction.requirements.Length)
            {
                case 1:
                    instructionIndicatorSprites[1].sprite = machineInstruction.requirements[0].element.sprite;
                    instructionIndicatorSprites[1].color = new Color(1,1,1,1);
                    instructionIndicatorTMPro[1].text = machineInstruction.requirements[0].amount.ToString();
                    break;
                case 2:
                    instructionIndicatorSprites[0].sprite = machineInstruction.requirements[0].element.sprite;
                    instructionIndicatorSprites[0].color = new Color(1, 1, 1, 1);
                    instructionIndicatorTMPro[0].text = machineInstruction.requirements[0].amount.ToString();

                    instructionIndicatorSprites[2].sprite = machineInstruction.requirements[1].element.sprite;
                    instructionIndicatorSprites[2].color = new Color(1, 1, 1, 1);
                    instructionIndicatorTMPro[2].text = machineInstruction.requirements[1].amount.ToString();
                    break;
                case 3:
                    for (int index = 0; index < 3; index++)
                    {
                        instructionIndicatorSprites[index].sprite = machineInstruction.requirements[index].element.sprite;
                        instructionIndicatorSprites[index].color = new Color(1,1,1,1);
                        instructionIndicatorTMPro[index].text = machineInstruction.requirements[index].amount.ToString();
                    }
                    break;
            }
            instructionIndicatorOutcomeSprite.sprite = machineInstruction.outcome.element.sprite;
            instructionIndicatorOutcomeTMPro.text = machineInstruction.outcome.amount.ToString();
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
