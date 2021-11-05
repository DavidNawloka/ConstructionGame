using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Core 
{
    public class InputAllowance : MonoBehaviour
    {
        [SerializeField] UnityEvent<bool> OnInputDeactivationChange;
        [SerializeField] UnityEvent<bool> OnZoomDeactivationChange;

        bool inputDisabled = false;
        bool zoomDisabled = false;

        public void ToggleInputAllowance()
        {
            inputDisabled = !inputDisabled;
            OnInputDeactivationChange.Invoke(inputDisabled);
        }
        public void DisableInput()
        {
            inputDisabled = true;
            OnInputDeactivationChange.Invoke(inputDisabled);
        }
        public void EnableInput()
        {
            inputDisabled = false;
            OnInputDeactivationChange.Invoke(inputDisabled);
        }



        public void ToggleZoomAllowance()
        {
            zoomDisabled = !zoomDisabled;
            OnZoomDeactivationChange.Invoke(zoomDisabled);
        }
        public void DisableZoom()
        {
            zoomDisabled = true;
            OnZoomDeactivationChange.Invoke(zoomDisabled);
        }
        public void EnableZoom()
        {
            zoomDisabled = false;
            OnZoomDeactivationChange.Invoke(zoomDisabled);
        }
        public void SetActiveZoom(bool isDisabled)
        {
            zoomDisabled = isDisabled;
            OnZoomDeactivationChange.Invoke(zoomDisabled);
        }


    }
}
