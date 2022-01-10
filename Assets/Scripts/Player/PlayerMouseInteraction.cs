using CON.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using System;
using System.Collections;

namespace CON.Player
{
    public class PlayerMouseInteraction : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera followCamera;
        [SerializeField] float maxCameraDistance = 25;
        [SerializeField] float minCameraDistance = 5;
        [Header("Cursor Sprites")]
        [SerializeField] CursorMapping[] cursorMappings;

        PlayerMovement playerMovement;
        CursorType currentCursorType;

        bool isZoomDisabled = false;
        bool isInputDisabled = false;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        private void Update()
        {
            if (UIInteraction()) return;

            if (!isZoomDisabled) ManageCameraZoom();
            

            RaycastHit[] raycastHits;
            if (!GetCameraRaycastHit(out raycastHits))
            {
                SetCursor(CursorType.None);
                return;
            }
            Array.Sort(raycastHits, null, new RaycastHitSorter());

            if (WorldInteraction(raycastHits)) return;

            if (MovementInteraction(raycastHits)) return;
        }

        public void OnZoomDeactivationChange(bool isDisabled) // Input Allowance Class Event
        {
            isZoomDisabled = isDisabled;
        }
        public void OnInputDeactivationChange(bool isDisabled) // Input Allowance Class Event
        {
            isInputDisabled = isDisabled;
        }
        private void ManageCameraZoom()
        {
            int factor = 0;
            if (Input.mouseScrollDelta.y > 0)
            {
                factor = -1;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                factor = 1;
            }
            else return;

            CinemachineComponentBase componentBase = followCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = Mathf.Clamp((componentBase as CinemachineFramingTransposer).m_CameraDistance + 2 * factor, minCameraDistance, maxCameraDistance);
            }
        }

        private bool UIInteraction()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }
        private bool WorldInteraction(RaycastHit[] raycastHits)
        {
            IRaycastable interactable = null;
            foreach (RaycastHit raycastHit in raycastHits)
            {
                interactable = raycastHit.transform.GetComponent<IRaycastable>();
                if (interactable != null) break;
            }
            if (interactable == null) return false;

            if (interactable.InRange(transform))
            {
                SetCursor(interactable.GetCursorType());

                if (Input.GetMouseButtonDown(0))
                {
                    interactable.HandleInteractionClick(transform);
                }

                return true;
            }

            return false;
        }

        private bool MovementInteraction(RaycastHit[] raycastHits)
        {
            if (playerMovement.MoveTo(raycastHits))
            {
                SetCursor(CursorType.Movement);
                return true;
            }
            else
            {
                SetCursor(CursorType.None);
                return false;
            }
        }

        private void SetCursor(CursorType cursorType)
        {
            if (currentCursorType == cursorType) return;

            currentCursorType = cursorType;

            CursorMapping cursorMapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(cursorMapping.sprite, cursorMapping.hotspot, CursorMode.Auto);
        }
        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping cursorMapping in cursorMappings)
            {
                if (cursorMapping.cursorType == cursorType)
                {
                    return cursorMapping;
                }
            }
            Debug.LogError("This cursor type was not found in the 'cursorMappings' array: " + cursorType);
            return cursorMappings[0];
        }

        private bool GetCameraRaycastHit(out RaycastHit[] raycastHits)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            raycastHits = Physics.RaycastAll(cameraRay);
            if (raycastHits.Length == 0)
            {
                return false;
            }
            return true;
        }

        class RaycastHitSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                RaycastHit hit1 = (RaycastHit)x;
                RaycastHit hit2 = (RaycastHit)y;

                if (hit1.distance < hit2.distance) return -1;
                else if (hit1.distance > hit2.distance) return 1;
                else return 0;
            }
        }

        [System.Serializable]
        public class CursorMapping
        {
            public CursorType cursorType;
            public Texture2D sprite;
            public Vector2 hotspot;
        }

    }

}