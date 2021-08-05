using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Materials
{

    public class MaterialPickup : MonoBehaviour
    {
        [SerializeField] Material material;
        [SerializeField] int amount = 1;
        private void OnTriggerEnter(Collider other) // Consider having to click on it
        {

        }
    }

}