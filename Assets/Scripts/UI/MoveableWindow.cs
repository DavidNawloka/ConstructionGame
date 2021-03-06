using CON.Core;
using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CON.UI
{
    public class MoveableWindow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Connectors")]
        [SerializeField] RectTransform horizontalConnection;
        [SerializeField] RectTransform verticalConnection;
        [Header("Window")]
        [SerializeField] float windowWidth;
        [SerializeField] float windowHeight;

        CloseButtonManager escManager;
        Transform connectTo;
        CanvasGroup canvasGroup;
        bool followMouse = false;
        Vector3 initialMousePosition;
        bool isHidden = true;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            escManager = FindObjectOfType<CloseButtonManager>();
        }
        private void Start()
        {
            SetActiveCanvas(false,null);
        }
        void Update()
        {
            if (isHidden) return;

            UpdateOwnPosition();
            UpdateConnectionPosition();
        }
        public bool IsShown()
        {
            return !isHidden;
        }
        public void ToggleCanvas(Transform connectTo)
        {
            SetActiveCanvas(isHidden,connectTo);
        }
        public void SetActiveCanvas(bool isActive, Transform connectTo)
        {
            this.connectTo = connectTo;

            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
            horizontalConnection.gameObject.SetActive(isActive);
            verticalConnection.gameObject.SetActive(isActive);

            isHidden = !isActive;

            if (isActive)
            {

                transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0) + new Vector3(Random.Range(Screen.width * -.2f, Screen.width*.2f), Random.Range(Screen.height * -.2f, Screen.height * .2f));
                UpdateConnectionPosition();
                escManager.AddFunction(() => SetActiveCanvas(false, null), gameObject.GetHashCode().ToString());
                canvasGroup.alpha = 1;
            }
            else
            {
                escManager.RemoveFunction(gameObject.GetHashCode().ToString());
                canvasGroup.alpha = 0;
            }
        }
        private void UpdateOwnPosition()
        {
            if (followMouse)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(Input.mousePosition.x + initialMousePosition.x, windowWidth/2, Screen.currentResolution.width-windowWidth/2),
                    Mathf.Clamp(Input.mousePosition.y + initialMousePosition.y, windowHeight/2, Screen.currentResolution.height-windowHeight/2),
                    0);
            }
        }
        private void UpdateConnectionPosition()
        {
            Vector3 machineScreenSpacePosition = Camera.main.WorldToScreenPoint(connectTo.transform.position);
            Vector3 posDifference = transform.position - machineScreenSpacePosition;


            horizontalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.x), horizontalConnection.sizeDelta.y);
            verticalConnection.sizeDelta = new Vector2(Mathf.Abs(posDifference.y), verticalConnection.sizeDelta.y);

            horizontalConnection.position = new Vector3((posDifference.x / 2) + machineScreenSpacePosition.x, transform.position.y);

            verticalConnection.position = new Vector3(machineScreenSpacePosition.x, (posDifference.y / 2) + machineScreenSpacePosition.y);
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
            followMouse = false;
        }
    }

}