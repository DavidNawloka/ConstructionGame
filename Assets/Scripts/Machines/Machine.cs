using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines 
{
    public class Machine : MonoBehaviour, IPlaceable
    {
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] Element elementPlacementRequirement;
        [SerializeField] InventoryItem[] elementBuildingRequirements;

        NavMeshObstacle navMeshObstacle;

        void Awake()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();
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
        }

        public InventoryItem[] GetNeededBuildingElements()
        {
            return elementBuildingRequirements;
        }
    }
}