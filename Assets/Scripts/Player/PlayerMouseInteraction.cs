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
            if (MovementInteraction()) return;
        }

        private bool UIInteraction()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return true;
            return false;
        }

        private bool MovementInteraction()
        {
            if (!Input.GetMouseButton(0)) return false;

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            return playerMovement.MoveTo(cameraRay);
        }
    }

}