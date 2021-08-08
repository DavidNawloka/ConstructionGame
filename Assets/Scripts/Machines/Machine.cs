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
        [SerializeField] Element elementRequirement;

        NavMeshObstacle navMeshObstacle;

        void Awake()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        void Update()
        {

        }
        public Element GetElementRequirement()
        {
            return elementRequirement;
        }
        public Vector2Int[] GetTakenGridPositions()
        {
            return takenGridPositions;
        }
        public void FullyPlaced()
        {
            navMeshObstacle.enabled = true;
        }
    }
}