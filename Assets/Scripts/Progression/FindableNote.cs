using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace CON.Progression
{
    public class FindableNote : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        bool followMouse = false;
        Vector3 initialMousePosition;

        private void Update()
        {
            if (!followMouse) return;
            UpdateOwnPosition();
        }
        private void UpdateOwnPosition()
        {
            transform.position = new Vector3(
                    Input.mousePosition.x + initialMousePosition.x,
                    Input.mousePosition.y + initialMousePosition.y,
                    0);
        }
        // Interface Implementations
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                followMouse = true;
                initialMousePosition = transform.position - Input.mousePosition;
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