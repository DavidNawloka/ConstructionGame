using CON.BuildingGrid;
using CON.Core;
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
        [SerializeField] AudioClip ToggleBuildModeSound;
        [SerializeField] AudioClip ToggleDemolishModeSound;

        CanvasGroup canvasGroup;
        BuildingGridMesh buildingGridMesh;
        Builder builder;
        AudioSourceManager audioSourceManager;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            builder = FindObjectOfType<Builder>();
            audioSourceManager = GetComponent<AudioSourceManager>();
            buildingGridMesh = FindObjectOfType<BuildingGridMesh>();
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
            audioSourceManager.PlayOnce(ToggleDemolishModeSound);
        }
        private void SetCanvasGroup(bool isActive)
        {
            audioSourceManager.PlayOnce(ToggleBuildModeSound);
            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
            buildingGridMesh.SetActiveMesh(isActive);

            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;
        }
    }
}
