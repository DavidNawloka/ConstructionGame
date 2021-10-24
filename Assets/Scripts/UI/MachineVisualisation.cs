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
    public class MachineVisualisation : MonoBehaviour
    {
        [Header("Inventory")]
        [SerializeField] InventoryItemVisualisation[] inventoryRequirmentVisualiser;
        [SerializeField] TextMeshProUGUI[] inventorySlotTMPro;
        [SerializeField] RectTransform sliderForeground;
        [Header("Machine specific")]
        [SerializeField] Image[] requirementImages;
        [SerializeField] Image outcomeImage;
        [Header("Instruction related")]
        [SerializeField] TMP_Dropdown instructionDropdown;
        [SerializeField] Image[] instructionIndicatorSprites;
        [SerializeField] TextMeshProUGUI[] instructionIndicatorTMPro;
        [SerializeField] Image instructionIndicatorOutcomeSprite;
        [SerializeField] TextMeshProUGUI instructionIndicatorOutcomeTMPro;

        
        Machine machine;
        MoveableWindow moveableWindow;

        private void Awake()
        {
            machine = GetComponentInParent<Machine>();
            moveableWindow = GetComponent<MoveableWindow>();
        }
        private void Start()
        {
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

                if (inventory.GetAmountOfElement(machine.GetCurrentInstruction().requirements[requirementIndex].element) > 0)
                {
                    inventoryRequirmentVisualiser[requirementIndex].image.enabled = false;
                    inventoryRequirmentVisualiser[requirementIndex].tmPro.enabled = false;
                }
                else
                {
                    inventoryRequirmentVisualiser[requirementIndex].image.enabled = true;
                    inventoryRequirmentVisualiser[requirementIndex].tmPro.enabled = true;
                }
            }
        }

        public void AddEnergy() // Button Function
        {
            machine.AddAllEnergyElements();
        }

        public void MachineClicked() // Event from Machine Class function
        {
            transform.position = Camera.main.WorldToScreenPoint(machine.transform.position);
            moveableWindow.ToggleCanvas(machine.transform);
        }

        

        private void SetActiveElementIndicators(bool isActive)
        {
            foreach (Image requirementImage in requirementImages)
            {
                requirementImage.enabled = isActive;
            }
            outcomeImage.enabled = isActive;
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
            inventoryRequirmentVisualiser[1].image.enabled = isNeeded;
            inventoryRequirmentVisualiser[1].tmPro.enabled = isNeeded;

            isNeeded = machineInstruction.requirements.Length >= 3;
            inventoryRequirmentVisualiser[2].image.enabled = isNeeded;
            inventoryRequirmentVisualiser[2].tmPro.enabled = isNeeded;

            for (int inventoryIndex = 0; inventoryIndex < machineInstruction.requirements.Length; inventoryIndex++)
            {
                inventoryRequirmentVisualiser[inventoryIndex].image.sprite = machineInstruction.requirements[inventoryIndex].element.sprite;
                inventoryRequirmentVisualiser[inventoryIndex].tmPro.text = machineInstruction.requirements[inventoryIndex].amount.ToString();

                requirementImages[inventoryIndex].sprite = machineInstruction.requirements[inventoryIndex].element.sprite;
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

        
    }
}
