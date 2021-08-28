using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CON.UI
{
    public class BuilderVisualisation : MonoBehaviour
    {
        [SerializeField] Image demolishModeOverlayImage;

        CanvasGroup canvasGroup;
        BuildingGridMesh buildingGridMesh;
        Builder builder;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            builder = FindObjectOfType<Builder>();
            buildingGridMesh = builder.GetGridMesh();
        }
        private void OnEnable()
        {
            builder.onDemolishModeChange += OnDemolishModeChange;
            builder.onBuildModeChange += SetCanvasGroup;
        }
        public void ToggleBuildMode() // Button onClick event function
        {
            builder.ToggleBuildMode();
        }
        public void ToggleDemolishMode() // Button onClick event function
        {
            builder.ToggleDemolishMode();
        }
        private void OnDemolishModeChange(bool isActive) // Builder class event function
        {
            demolishModeOverlayImage.gameObject.SetActive(isActive);
        }
        private void SetCanvasGroup(bool isActive)
        {
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
            buildingGridMesh.ToggleMesh(isActive);

            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;
        }
    }
}
