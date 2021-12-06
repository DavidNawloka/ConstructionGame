using CON.Core;
using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CON.UI
{
    public class MoveableWindowNoCon : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] float minX;
        [SerializeField] float maxX;
        [SerializeField] float minY;
        [SerializeField] float maxY;
        Vector3 initialMousePosition;
        Vector3 initialTransformPosition;
        bool followMouse = false;

        RectTransform rectTransform;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            initialMousePosition = rectTransform.anchoredPosition3D;
        }

        void Update()
        {
            if (followMouse) UpdateOwnPosition();
        }
        private void UpdateOwnPosition()
        {
            transform.localPosition = new Vector3(
                    Mathf.Clamp(Input.mousePosition.x + initialMousePosition.x, minX, maxX),
                    Mathf.Clamp(Input.mousePosition.y + initialMousePosition.y,minY, maxY),
                    0);
        }

        // Interface Implementations
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                followMouse = true;
                initialMousePosition = transform.localPosition - Input.mousePosition;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                followMouse = false;
            }
        }
    }

}