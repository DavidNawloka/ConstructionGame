using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Materials
{
    [CreateAssetMenu(fileName ="Resource",menuName ="Resources/Create New Resource")]
    public class Material : ScriptableObject
    {
        public Sprite sprite;
        public GameObject pickupPrefab;
        public float minScale;
        public float maxScale;
    }
}
