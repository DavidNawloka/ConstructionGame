using CON.BuildingGrid;
using CON.Core;
using CON.Machines;
using CON.Progression;
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
        [SerializeField] PlaceableButton placeableButtonPrefab;
        [Header("Button Tabs")]
        [SerializeField] GameObject conveyorTab;
        [SerializeField] GameObject gathererTab;
        [SerializeField] GameObject crafterTab;

        CanvasGroup canvasGroup;
        BuildingGridMesh buildingGridMesh;
        Builder builder;
        AudioSourceManager audioSourceManager;
        ProgressionManager progressionManager;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            builder = FindObjectOfType<Builder>();
            audioSourceManager = GetComponent<AudioSourceManager>();
            buildingGridMesh = FindObjectOfType<BuildingGridMesh>();
            progressionManager = FindObjectOfType<ProgressionManager>();

            builder.onDemolishModeChange += OnDemolishModeChange;
            builder.onBuildModeChange += SetCanvasGroup;
            progressionManager.OnPlaceableUnlocked.AddListener(UnlockPlaceable);
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
        private void UnlockPlaceable(Unlockable newPlaceable)
        {
            PlaceableButton buttonInstance = Instantiate(placeableButtonPrefab);
            buttonInstance.InitialiseButton(newPlaceable);

            switch (newPlaceable.type)
            {
                case PlaceableType.Conveyor:
                    buttonInstance.transform.SetParent(conveyorTab.transform);
                    break;
                case PlaceableType.Gatherer:
                    buttonInstance.transform.SetParent(gathererTab.transform);
                    break;
                case PlaceableType.Crafter:
                    buttonInstance.transform.SetParent(crafterTab.transform);
                    break;
            }
            buttonInstance.GetComponent<RectTransform>().localScale = Vector3.one;
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
