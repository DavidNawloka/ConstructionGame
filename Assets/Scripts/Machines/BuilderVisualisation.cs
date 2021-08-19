using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Machines
{
    public class BuilderVisualisation : MonoBehaviour
    {
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
        public void ToggleBuildModeUI()
        {
            SetCanvasGroup(!canvasGroup.interactable);
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
