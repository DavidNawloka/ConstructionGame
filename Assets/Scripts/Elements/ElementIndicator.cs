using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{
    [ExecuteInEditMode]
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
            Gizmos.matrix = Matrix4x4.TRS(this.transform.TransformPoint(GetComponent<BoxCollider>().center), this.transform.rotation, this.transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, GetComponent<BoxCollider>().size);
        }
    }
}
