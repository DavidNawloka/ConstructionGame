using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System;
using Astutos.Saving;
using CON.Machines;
using CON.UI;

namespace CON.Progression
{
    public class ProgressionNode : MonoBehaviour, ISaveable
    {
        public bool unlocked = false;
        public bool unlockedView = false;
        [SerializeField] ProgressionNode[] parentNodes;
        [SerializeField] InventoryItemVisualisation[] requirementVisualisation;
        [SerializeField] Unlockable unlockable;
        [SerializeField] GameObject nodeConnector;
        [SerializeField] Transform connectorsParent;
        [Header("Unlockable Visualisation")]
        [SerializeField] TextMeshProUGUI unlockableName;
        [SerializeField] Image unlockableImage;
        [SerializeField] TextMeshProUGUI unlockableDescription;
        [SerializeField] Button unlockButton;
        [SerializeField] Animation lockedViewAnimation;
        [SerializeField] InstructionVisualisation[] instructionVisualisations;
        [SerializeField] Color unlockedMainBackgroundColor;
        [SerializeField] Color unlockedBorderColor;
        [SerializeField] Image border;
        [SerializeField] Color unlockedInstructionVisualisationColor;
        [SerializeField] Image instructionVisualisationBackground;
        [SerializeField] GameObject helpWindow;
        [SerializeField] GameObject instructionWindow;

        ProgressionManager progressionManager;
        

        private void Awake()
        {
            progressionManager = FindObjectOfType<ProgressionManager>();
            unlockButton.onClick.AddListener(OnClick);
            progressionManager.OnMachineProducedElement.AddListener(CheckIfEnoughElementsProduced);
            progressionManager.OnPlaceableUnlocked.AddListener(CheckUnlockView);
        }
        private void Start()
        {
            UpdateUnlockableVisualisation();
            progressionManager.OnMachineProducedElement.Invoke(progressionManager.GetInventory());
        }
        public void UpdateUnlockableVisualisation()
        {
            if (unlockable.prefab == null) return;
            unlockableName.text = unlockable.name;
            unlockableImage.sprite = unlockable.sprite;
            unlockableDescription.text = unlockable.description;
            UpdateRequirements();
            
            Machine machine = unlockable.prefab.GetComponent<Machine>();
            if (machine != null) UpdateInstructionVisualisations(machine);
            else helpWindow.gameObject.SetActive(true);
        }

        private void UpdateInstructionVisualisations(Machine machine)
        {
            instructionWindow.gameObject.SetActive(true);
            if (machine == null) return;
            for (int instructionIndex = 0; instructionIndex < instructionVisualisations.Length; instructionIndex++)
            {
                if (instructionIndex >= machine.GetPossibleInstructions().Length)
                {
                    instructionVisualisations[instructionIndex].gameObject.SetActive(false);
                    continue;
                }
                instructionVisualisations[instructionIndex].UpdateInstruction(machine.GetPossibleInstructions()[instructionIndex]);
            }
        }

        private void UpdateRequirements()
        {
            for (int index = 0; index < requirementVisualisation.Length; index++)
            {
                if(index >= unlockable.elementRequirements.Length)
                {
                    requirementVisualisation[index].tmPro.transform.parent.gameObject.SetActive(false);
                    continue;
                }
                requirementVisualisation[index].tmPro.text = "0 / " + unlockable.elementRequirements[index].amount.ToString();
                requirementVisualisation[index].image.sprite = unlockable.elementRequirements[index].element.sprite;
            }
        }

        private void CheckIfEnoughElementsProduced(Inventory inventory)
        {
            if(!unlocked) unlockButton.interactable = inventory.HasItem(unlockable.elementRequirements);
            for (int index = 0; index < requirementVisualisation.Length; index++)
            {
                if (!requirementVisualisation[index].tmPro.transform.gameObject.activeInHierarchy)
                {
                    continue;
                }

                int amountOfElementProduced = inventory.GetAmountOfElement(unlockable.elementRequirements[index].element);
                requirementVisualisation[index].tmPro.text = GetFormattedElementProduced(amountOfElementProduced) + " / " + unlockable.elementRequirements[index].amount.ToString();

                if(amountOfElementProduced >= unlockable.elementRequirements[index].amount)
                {
                    requirementVisualisation[index].tmPro.color = Color.green;
                }
                else
                {
                    requirementVisualisation[index].tmPro.color = Color.red;
                }
            }
        }

        private string GetFormattedElementProduced(int elementProduced)
        {
            string stringAmount = elementProduced.ToString();
            if(stringAmount.Length > 3)
            {
                stringAmount = stringAmount.Substring(0, 1) + "k";
            }
            return stringAmount;
        }

        private void OnClick()
        {
            unlocked = true;
            progressionManager.UnlockPlaceable(unlockable);
            GetComponent<Image>().color = unlockedMainBackgroundColor;
            border.color = unlockedBorderColor;
            instructionVisualisationBackground.color = unlockedInstructionVisualisationColor;
            unlockButton.interactable = false;
            progressionManager.OnPlaceableUnlocked.RemoveListener(CheckUnlockView);
        }

        private void CheckUnlockView(Unlockable unlockedPlaceable)
        {
            foreach (ProgressionNode parentNode in parentNodes)
            {
                if (parentNode.unlocked && !unlockedView)
                {
                    lockedViewAnimation.Play();
                    unlockedView = true;
                }
            }
        }


#if UNITY_EDITOR
        public void InstantiateConnectors()
        {
            CheckUnlockView(null);
            foreach (ProgressionNode parentNode in parentNodes)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(nodeConnector);
                instance.transform.SetParent(connectorsParent);
                instance.transform.SetAsFirstSibling();
                

                Vector3 distance = parentNode.transform.position - transform.position;

                RectTransform instanceTransform = instance.GetComponent<RectTransform>();

                instanceTransform.sizeDelta = new Vector2(distance.magnitude, instanceTransform.sizeDelta.y);
                instanceTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan(distance.y / distance.x) * Mathf.Rad2Deg);
                instanceTransform.position = transform.position + distance / 2;
            }
        }
#endif
        public object CaptureState()
        {
            return unlocked;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;
            unlocked = (bool)state;
            if (unlocked && unlockable.prefab != null)
            {
                lockedViewAnimation.Play();
                OnClick();
            }
        }

    }

}