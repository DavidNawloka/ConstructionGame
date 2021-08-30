using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CON.Elements;
using CON.Machines;

namespace CON.UI
{
    public class PlaceableButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buttonHead;
        [SerializeField] TextMeshProUGUI[] requirementAmounts;
        [SerializeField] Image[] requirementSprites;
        [SerializeField] GameObject placeablePrefab;

        Transform player;
        Button button;
        BuilderVisualisation builderVisualisation;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            button = GetComponent<Button>();
            builderVisualisation = GetComponentInParent<BuilderVisualisation>();
        }
        private void OnEnable()
        {
            player.GetComponent<Inventory>().OnInventoryChange.AddListener(UpdateRequirements);
            button.onClick.AddListener(OnClick);
        }
        private void Start()
        {
            buttonHead.text = placeablePrefab.name;
        }
        private void UpdateRequirements(Inventory playerInventory)
        {
            IPlaceable placeable = placeablePrefab.GetComponent<IPlaceable>();
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
            player.GetComponent<Builder>().SetActiveDemolishMode(false);
        }
    }

}