using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines
{
    public class Conveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] Vector2Int[] takenGridPositions;

        NavMeshObstacle navMeshObstacle;

        void Awake()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();   
        }

        // Update is called once per frame
        void Update()
        {

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