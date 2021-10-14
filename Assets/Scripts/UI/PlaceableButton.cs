using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CON.Elements;
using CON.Machines;
using CON.Core;

namespace CON.UI
{
    public class PlaceableButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buttonHead;
        [SerializeField] TextMeshProUGUI[] requirementAmounts;
        [SerializeField] Image[] requirementSprites;
        [SerializeField] GameObject placeablePrefab;
        [SerializeField] Image elementPlacementIndicator;
        [SerializeField] AudioClip clickSound;

        Transform player;
        Button button;
        IPlaceable placeable;
        AudioSourceManager audioSourceManager;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            button = GetComponent<Button>();
            placeable = placeablePrefab.GetComponent<IPlaceable>();
            audioSourceManager = GetComponent<AudioSourceManager>();
        }
        private void OnEnable()
        {
            player.GetComponent<Inventory>().OnInventoryChange.AddListener(UpdateRequirements);
            button.onClick.AddListener(OnClick);
            UpdateRequirements(player.GetComponent<Inventory>());
        }
        private void Start()
        {
            if (placeable.GetElementPlacementRequirement() == null) elementPlacementIndicator.color = Color.white;
            else elementPlacementIndicator.color = placeable.GetElementPlacementRequirement().colorRepresentation;
            
        }
        private void UpdateRequirements(Inventory playerInventory)
        {
            InventoryItem[] requirements = placeable.GetNeededBuildingElements();

            bool hasAllItems = true;

            for (int index = 0; index < requirements.Length; index++)
            {
                if (!playerInventory.HasItem(requirements[index]))
                {
                    requirementAmounts[index].color = Color.red;
                    hasAllItems = false;
                }
                else
                {
                    requirementAmounts[index].color = Color.white;
                }
                requirementAmounts[index].text = requirements[index].amount.ToString();
                requirementSprites[index].sprite = requirements[index].element.sprite;
            }

            if (hasAllItems) 
            { 
                buttonHead.color = Color.white;
                button.interactable = true;
            }
            else
            {
                buttonHead.color = Color.red;
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