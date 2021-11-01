using CON.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;

namespace CON.Player
{
    public class PlayerMouseInteraction : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera followCamera;
        [SerializeField] float maxCameraDistance = 25;
        [SerializeField] float minCameraDistance = 5;


        PlayerMovement playerMovement;


        bool isZoomDisabled = false;
        bool isInputDisabled = false;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        private void Update()
        {
            if (isInputDisabled) return;

            if(!isZoomDisabled) ManageCameraZoom();
            if (UIInteraction()) return;

            
            RaycastHit raycastHit;
            if (!GetCameraRaycastHit(out raycastHit)) return;

            if (WorldInteraction(raycastHit)) return;
            if (MovementInteraction(raycastHit)) return;
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
            if (EventSystem.current.IsPointerOverGameObject()) return true;
            return false;
        }
        private bool WorldInteraction(RaycastHit raycastHit)
        {
            bool status = false;

            IMouseClickable interactable = raycastHit.transform.GetComponent<IMouseClickable>();
            if (interactable == null) return status;


            if (Input.GetMouseButtonDown(0))
            {
                status = interactable.HandleInteractionClick(transform);
            }

            return status;
        }

        private bool MovementInteraction(RaycastHit raycastHit)
        {
            if (!Input.GetMouseButton(1)) return false;
            return playerMovement.MoveTo(raycastHit);
        }

        private bool GetCameraRaycastHit(out RaycastHit raycastHit)
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out raycastHit))
            {
                return true;
            }
            return false;
        }

    }

}