using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Machines
{
    public class BuilderVisualisation : MonoBehaviour
    {
        public UnityEvent<bool> OnBuildModeChange;

        CanvasGroup canvasGroup;
        BuildingGridMesh buildingGridMesh;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            buildingGridMesh = FindObjectOfType<BuildingGridMesh>();
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
        public void ToggleBuildModeUI()
        {
            SetCanvasGroup(!canvasGroup.interactable);
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
