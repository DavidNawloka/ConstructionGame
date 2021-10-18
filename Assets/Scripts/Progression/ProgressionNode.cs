using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System;

namespace CON.Progression
{
    public class ProgressionNode : MonoBehaviour
    {
        public bool unlocked = false;
        [SerializeField] ProgressionNode[] parentNodes;
        [SerializeField] ElementRequirementVisualisation[] requirementVisualisation;
        [SerializeField] Unlockable unlockable;
        [SerializeField] GameObject nodeConnector;
        [Header("Unlockable Visualisation")]
        [SerializeField] TextMeshProUGUI unlockableName;
        [SerializeField] Image unlockableImage;
        [SerializeField] TextMeshProUGUI unlockableDescription;
        [SerializeField] Button unlockButton;
        [SerializeField] GameObject lockedView;

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
            UpdateRequirements();
        }
        private void UpdateUnlockableVisualisation()
        {
            unlockableName.text = unlockable.name;
            unlockableImage.sprite = unlockable.sprite;
            unlockableDescription.text = unlockable.description;
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

        private void CheckIfEnoughElementsProduced(Inventory inventory) // doesn't work when there are less than 3 requirements
        {
            unlockButton.interactable = inventory.HasItem(unlockable.elementRequirements);
            for (int index = 0; index < requirementVisualisation.Length; index++)
            {
                if (!requirementVisualisation[index].tmPro.transform.gameObject.activeInHierarchy)
                {
                    print("hello");
                    continue;
                }
                requirementVisualisation[index].tmPro.text = inventory.GetAmountOfElement(unlockable.elementRequirements[index].element) + " / " + unlockable.elementRequirements[index].amount.ToString();
            }
        }

        private void OnClick()
        {
            unlocked = true;
            progressionManager.UnlockPlaceable(unlockable);
        }

        private void CheckUnlockView(List<Unlockable> unlockedPlaceables)
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


        [System.Serializable]
        class ElementRequirementVisualisation
        {
            public Image image; 
            public TextMeshProUGUI tmPro; 
        }
    }

}