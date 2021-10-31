using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System;
using Astutos.Saving;

namespace CON.Progression
{
    public class ProgressionNode : MonoBehaviour, ISaveable
    {
        public bool unlocked = false;
        [SerializeField] ProgressionNode[] parentNodes;
        [SerializeField] InventoryItemVisualisation[] requirementVisualisation;
        [SerializeField] Unlockable unlockable;
        [SerializeField] GameObject nodeConnector;
        [Header("Unlockable Visualisation")]
        [SerializeField] TextMeshProUGUI unlockableName;
        [SerializeField] Image unlockableImage;
        [SerializeField] TextMeshProUGUI unlockableDescription;
        [SerializeField] Button unlockButton;
        [SerializeField] GameObject lockedView;
        [SerializeField] Color unlockedBackgroundColor;

        ProgressionManager progressionManager;

        private void Awake()
        {
            progressionManager = GetComponentInParent<ProgressionManager>();
            unlockButton.onClick.AddListener(OnClick);
        }
        private void OnEnable()
        {
            progressionManager.OnMachineProducedElement.AddListener(CheckIfEnoughElementsProduced);
            progressionManager.OnPlaceableUnlocked.AddListener(CheckUnlockView);
        }
        private void Start()
        {
            UpdateUnlockableVisualisation();
        }
        public void UpdateUnlockableVisualisation()
        {
            unlockableName.text = unlockable.name;
            unlockableImage.sprite = unlockable.sprite;
            unlockableDescription.text = unlockable.description;
            UpdateRequirements();
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
                requirementVisualisation[index].tmPro.text = amountOfElementProduced + " / " + unlockable.elementRequirements[index].amount.ToString();

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

        private void OnClick()
        {
            unlocked = true;
            progressionManager.UnlockPlaceable(unlockable);
            GetComponent<Image>().color = unlockedBackgroundColor;
            unlockButton.interactable = false;
            progressionManager.OnPlaceableUnlocked.RemoveListener(CheckUnlockView);
        }

        private void CheckUnlockView(Unlockable unlockedPlaceable)
        {
            foreach (ProgressionNode parentNode in parentNodes)
            {
                if (parentNode.unlocked)
                {
                    lockedView.SetActive(false);
                }
            }
        }


#if UNITY_EDITOR
        public void InstantiateConnectors()
        {
            foreach(ProgressionNode parentNode in parentNodes)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(nodeConnector);
                instance.transform.SetParent(transform.parent.parent);
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
                lockedView.SetActive(false);
                OnClick();
            }
        }

    }

}