using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CON.Elements;

namespace CON.Machines
{
    public class MachineVisualisation : MonoBehaviour
    {
        [SerializeField] Image requirementSprite;
        [SerializeField] TextMeshProUGUI requirementTMPro;
        [SerializeField] RectTransform sliderForeground;

        RectTransform rectTransform;
        Transform mainCamera;
        CanvasGroup canvasGroup;
        Machine machine;

        bool isFirstClick;
        bool isHidden = true;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            machine = GetComponentInParent<Machine>();
            mainCamera = GameObject.FindGameObjectWithTag("FollowCamera").transform;
        }
        private void Start()
        {
            SetCanvasGroup(false);
            UpdateRequirementUI();
        }

        private void Update()
        {
            if (isHidden) return;

            rectTransform.LookAt(mainCamera.position);
            sliderForeground.localScale = new Vector3(machine.GetProductionFraction(), 1, 1);
        }

        public void MachineClicked()
        {
            if (isFirstClick) SetCanvasGroup(true);
            else SetCanvasGroup(false);
        }
        private void UpdateRequirementUI()
        {
            InventoryItem energyRequirement = machine.GetEnergyRequirement();
            requirementSprite.sprite = energyRequirement.element.sprite;
            requirementTMPro.text = energyRequirement.amount.ToString();
        }

        public void SetCanvasGroup(bool isActive)
        {
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;

            isFirstClick = !isActive;
            isHidden = !isActive;

            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;
        }
    }
}
