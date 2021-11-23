using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace CON.Elements
{
    public class ElementSpawner  : MonoBehaviour
    {
        [SerializeField] Element[] resources;
        [SerializeField] int amountOfResources;
        [SerializeField] int minPickupAmount = 1;
        [SerializeField] int maxPickupAmount = 3;
        [Header("Spawn Location")]
        [SerializeField] float minX;
        [SerializeField] float maxX;
        [SerializeField] float minY;
        [SerializeField] float maxY;
        [SerializeField] float minZ;
        [SerializeField] float maxZ;

        public void SpawnResources()
        {
            for (int i = 0; i < amountOfResources; i++)
            {
                Element resource = resources[UnityEngine.Random.Range(0, resources.Length)];
                SpawnResource(resource);
            }
        }
        public void DeleteResources()
        {
            foreach(GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup"))
            {
                if (pickup.transform.IsChildOf(transform))
                {
                    DestroyImmediate(pickup);
                }
                
            }
        }

        private void SpawnResource(Element resource)
        {
            Vector3 worldPosition = new Vector3(
                                UnityEngine.Random.Range(transform.position.x - minX, transform.position.x + maxX),
                                UnityEngine.Random.Range(transform.position.y - minY, transform.position.y + maxY),
                                UnityEngine.Random.Range(transform.position.z - minZ, transform.position.z + maxZ));
            Vector3 scale = new Vector3(
                UnityEngine.Random.Range(resource.minScale, resource.maxScale),
                UnityEngine.Random.Range(resource.minScale, resource.maxScale),
                UnityEngine.Random.Range(resource.minScale, resource.maxScale));

#if UNITY_EDITOR
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(resource.pickupPrefab);
            instance.transform.position = worldPosition;
            instance.transform.rotation = UnityEngine.Random.rotation;
            instance.transform.localScale = scale;
            instance.tag = "Pickup";
            instance.transform.parent = transform;
            instance.GetComponent<ElementPickup>().UpdateAmoutToEquip(UnityEngine.Random.Range(minPickupAmount, maxPickupAmount + 1));
#endif
        }

        private void OnDrawGizmosSelected()
        {

            Vector3 center = new Vector3(
                (transform.position.x + maxX + transform.position.x - minX) / 2,
                (transform.position.y + maxY + transform.position.y - minY) / 2,
                (transform.position.z + maxZ + transform.position.z - minZ) / 2);
            Vector3 size = new Vector3(
                            maxX + minX,
                            maxY + minY,
                            maxZ + minZ);
            Gizmos.DrawWireCube(center,size);
        }
    }
}
