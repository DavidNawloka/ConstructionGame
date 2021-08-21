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
        public UnityEvent<bool> OnBuildModeChange;

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
        }
        private void Start()
        {
            SetCanvasGroup(false);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBuildModeUI();
            }
        }
        public void ToggleBuildModeUI() // Button onClick event function
        {
            SetCanvasGroup(!canvasGroup.interactable);
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
            OnBuildModeChange.Invoke(isActive);

            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
            buildingGridMesh.ToggleMesh(isActive);

            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;
        }
    }
}
