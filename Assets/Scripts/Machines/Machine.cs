using CON.Elements;
using CON.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace CON.Machines 
{
    public class Machine : MonoBehaviour, IPlaceable
    {
        [Header("Placement")]
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] Element elementPlacementRequirement;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [Header("Production")]
        [SerializeField] ElementPickup elementToProduce;
        [SerializeField] float productionIntervall;
        [SerializeField] Transform elementExitPoint;

        NavMeshObstacle navMeshObstacle;
        bool fullyPlaced = false;

        float productionTimer = 0f;

        void Awake()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        private void Update()
        {
            if (!fullyPlaced) return;

            if(productionTimer >= productionIntervall)
            {
                productionTimer = 0;
                ProduceElement();
            }

            productionTimer += Time.deltaTime;
        }

        private void ProduceElement()
        {
            GameObject elementInstance = Instantiate(elementToProduce.gameObject, elementExitPoint.position, Quaternion.identity);
        }

        public Element GetElementRequirement()
        {
            return elementPlacementRequirement;
        }
        public Vector2Int[] GetTakenGridPositions()
        {
            return takenGridPositions;
        }
        public void FullyPlaced()
        {
            navMeshObstacle.enabled = true;
            fullyPlaced = true;
        }

        public InventoryItem[] GetNeededBuildingElements()
        {
            return elementBuildingRequirements;
        }

        private void OnMouseOver()
        {
            print("mouse over " + name);
        }
    }
}