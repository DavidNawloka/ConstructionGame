using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    public class PersistentGameobjectsSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentGameobjectsSpawnerPrefab;

        static bool hasSpawned;

        private void Awake()
        {
            if (!hasSpawned)
            {
                GameObject instance = Instantiate(persistentGameobjectsSpawnerPrefab);
                hasSpawned = true;
                DontDestroyOnLoad(instance);
            }
        }
    }

}