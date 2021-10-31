using CON.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using CON.Machines;

namespace CON.Player
{
    public class PlayerMouseInteraction : MonoBehaviour
    {
        [SerializeField] float maxCameraDistance = 25;
        [SerializeField] float minCameraDistance = 5;
        PlayerMovement playerMovement;
        CinemachineVirtualCamera followCamera;


        bool isZoomDisabled = false;
        bool isInputDisabled = false;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            followCamera = GameObject.FindGameObjectWithTag("FollowCamera").GetComponent<CinemachineVirtualCamera>();
        }
        private void Update()
        {
            if (isInputDisabled) return;

            if(!isZoomDisabled) ManageCameraZoom();
            if (UIInteraction()) return;
            if (WorldInteraction()) return;
            if (MovementInteraction()) return;
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
            if(Input.mouseScrollDelta.y > 0)
            {
                factor = -1;
            }
            if(Input.mouseScrollDelta.y < 0)
            {
                factor = 1;
            }

            CinemachineComponentBase componentBase = followCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_CameraDistance = Mathf.Clamp((componentBase as CinemachineFramingTransposer).m_CameraDistance + 2 * factor,minCameraDistance,maxCameraDistance);
            }
        }

        private bool UIInteraction()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return true;
            return false;
        }
        private bool WorldInteraction()
        {
            bool status = false;
            Ray cameraRay = GetCameraRay();
            RaycastHit raycastHit;
            if (Physics.Raycast(cameraRay, out raycastHit))
            {
                IMouseClickable interactable = raycastHit.transform.GetComponent<IMouseClickable>();
                if (interactable == null) return status;


                if (Input.GetMouseButtonDown(0))
                {
                    status = interactable.HandleInteractionClick(transform);
                }
            }
            return status;
        }

        private bool MovementInteraction()
        {
            if (!Input.GetMouseButton(1)) return false;
            Ray cameraRay = GetCameraRay();
            return playerMovement.MoveTo(cameraRay);
        }

        private static Ray GetCameraRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

    }

}