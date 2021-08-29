using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{

    [CreateAssetMenu(fileName ="Resource",menuName ="Resources/Create New Element")]
    [System.Serializable]
    public class Element : ScriptableObject
    {
        public Sprite sprite;
        public GameObject pickupPrefab;
        public float minScale;
        public float maxScale;
        public Color colorRepresentation;
    }
}
