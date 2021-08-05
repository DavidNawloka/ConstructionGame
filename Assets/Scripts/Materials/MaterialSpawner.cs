using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CON.Materials
{
    public class MaterialSpawner : MonoBehaviour
    {
        [SerializeField] Material[] resources;
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
                Material resource = resources[UnityEngine.Random.Range(0, resources.Length)];
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

        private void SpawnResource(Material resource)
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
                            (Mathf.Abs(minX) + Mathf.Abs(maxX)),
                            (Mathf.Abs(minY) + Mathf.Abs(maxY)),
                            (Mathf.Abs(minZ) + Mathf.Abs(maxZ)));
            Gizmos.DrawWireCube(center,size);
        }
    }
}
