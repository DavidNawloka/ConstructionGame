using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CON.Elements;
using CON.Machines;
using CON.Core;
using CON.Progression;

namespace CON.UI
{
    public class PlaceableButton : MonoBehaviour
    {

        [SerializeField] InventoryItemVisualisation[] requirementVisualisation;
        [SerializeField] AudioClip clickSound;
        [Header("Placeable Specific")]
        [SerializeField] TextMeshProUGUI placeableName;
        [SerializeField] GameObject placeablePrefab;
        [SerializeField] Image placeableImage;
        [SerializeField] Image elementPlacementIndicator;
        [SerializeField] GameObject multipleVersionsIndicator;

        Transform player;
        Button button;
        PlaceableInformation placeableInformation;
        AudioSourceManager audioSourceManager;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            button = GetComponent<Button>();
            audioSourceManager = GetComponent<AudioSourceManager>();
        }

        public void InitialiseButton(Unlockable newPlaceable)
        {
            placeablePrefab = newPlaceable.prefab;
            placeableName.text = newPlaceable.name;
            placeableImage.sprite = newPlaceable.sprite;

            placeableInformation = placeablePrefab.GetComponent<IPlaceable>().GetPlaceableInformation();

            if (placeableInformation.placementRequirement == null) elementPlacementIndicator.color = Color.white;
            else elementPlacementIndicator.color = placeableInformation.placementRequirement.colorRepresentation;

            UpdateRequirementsVisualisation(player.GetComponent<Inventory>());
            if (newPlaceable.hasMultipleVersions) multipleVersionsIndicator.SetActive(true);

            button.onClick.AddListener(OnClick);
            player.GetComponent<Inventory>().OnInventoryChange.AddListener(UpdateRequirementsVisualisation);
        }

        private void UpdateRequirementsVisualisation(Inventory playerInventory)
        {
            InventoryItem[] requirements = placeableInformation.buildingRequirements;

            bool hasAllItems = true;

            for (int index = 0; index < requirements.Length; index++)
            {
                if (!playerInventory.HasItem(requirements[index]))
                {
                    requirementVisualisation[index].tmPro.color = Color.red;
                    hasAllItems = false;
                }
                else
                {
                    requirementVisualisation[index].tmPro.color = Color.white;
                }
                requirementVisualisation[index].tmPro.text = requirements[index].amount.ToString();
                requirementVisualisation[index].image.sprite = requirements[index].element.sprite;
            }

            if (hasAllItems) 
            {
                placeableName.color = Color.white;
                button.interactable = true;
            }
            else
            {
                placeableName.color = Color.red;
                button.interactable = false;
            }
        }

        // Button Event Function
        private void OnClick()
        {
            player.GetComponent<Builder>().ActivatePlacementMode(placeablePrefab);
            audioSourceManager.PlayOnce(clickSound);
        }

    }

}