using CON.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CON.Player
{
    public class PlayerMouseInteraction : MonoBehaviour
    {
        PlayerMovement playerMovement;

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (UIInteraction()) return;
            if (WorldInteraction()) return;
            if (MovementInteraction()) return;
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