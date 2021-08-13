using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines
{
    public class Conveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [SerializeField] Vector3 boxCastHalfExtents;
        [SerializeField] float forceToApply;

        NavMeshObstacle navMeshObstacle;

        void Awake()
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();   
        }

        void Update()
        {
            //Collider[] hitColliders = Physics.OverlapBox(transform.position, boxCastHalfExtents, transform.rotation);
            //foreach(Collider collider in hitColliders)
            //{
            //    print(collider.name);
                
            //}
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
        private void OnCollisionStay(Collision collision)
        {
            ElementPickup elementPickup = collision.transform.GetComponentInParent<ElementPickup>();
            if (elementPickup == null) return;

            print("hello");
            collision.transform.GetComponentInParent<Rigidbody>().AddForce(-transform.right.normalized * forceToApply, ForceMode.Impulse);
        }
    }

}