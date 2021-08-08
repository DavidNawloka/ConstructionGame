using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{
    public class ElementIndicator : MonoBehaviour
    {
        [SerializeField] Element element;

        public Element GetElement()
        {
            return element;
        }

        private void OnDrawGizmos()
        {
            if (element == null) return;
            Gizmos.color = element.colorRepresentation;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
