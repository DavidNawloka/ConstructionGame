using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CON.Elements;

namespace CON.Machines
{
    public class PlaceableButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buttonHead;
        [SerializeField] TextMeshProUGUI[] requirementAmounts;
        [SerializeField] Image[] requirementSprites;
        [SerializeField] GameObject placeablePrefab;

        Transform player;
        Inventory playerInventory;
        Button button;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerInventory = player.GetComponent<Inventory>();
            button = GetComponent<Button>();
        }

        private void Start()
        {
            buttonHead.text = placeablePrefab.name;
            player.GetComponent<Inventory>().OnInventoryChange.AddListener(UpdateRequirements);
            button.onClick.AddListener(OnClick);
        }
        private void UpdateRequirements(InventoryItem[] inventory)
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

        private void OnClick()
        {
            player.GetComponent<Builder>().ActivateBuildMode(placeablePrefab);
        }
    }

}