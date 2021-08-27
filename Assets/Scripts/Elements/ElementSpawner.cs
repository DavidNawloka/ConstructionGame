using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CON.Elements
{
    public class ElementSpawner  : MonoBehaviour
    {
        [SerializeField] Element[] resources;
        [SerializeField] int amountOfResources;
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
                DestroyImmediate(pickup);
            }
        }

        private void SpawnResource(Element resource)
        {
            Vector3 worldPosition = new Vector3(
                                UnityEngine.Random.Range(minX, maxX),
                                UnityEngine.Random.Range(minY, maxY),
                                UnityEngine.Random.Range(minZ, maxZ));
            Vector3 scale = new Vector3(
                UnityEngine.Random.Range(resource.minScale, resource.maxScale),
                UnityEngine.Random.Range(resource.minScale, resource.maxScale),
                UnityEngine.Random.Range(resource.minScale, resource.maxScale));

            
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(resource.pickupPrefab);
            instance.transform.position = worldPosition;
            instance.transform.rotation = UnityEngine.Random.rotation;
            instance.transform.localScale = scale;
            instance.tag = "Pickup";
            instance.transform.parent = transform;
        }
        private void OnDrawGizmosSelected()
        {

            Vector3 center = new Vector3(
                            (minX + maxX) / 2,
                            (minY + maxY) / 2,
                            (minZ + maxZ) / 2);
            Vector3 size = new Vector3(
                            (Mathf.Abs(maxX) - Mathf.Abs(minX)),
                            (Mathf.Abs(maxY) - Mathf.Abs(minY)),
                            (Mathf.Abs(maxZ) - Mathf.Abs(minZ)));
            Gizmos.DrawWireCube(center,size);
        }
    }
}
