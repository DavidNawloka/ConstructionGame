using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    interface IMouseInteractable
    {
        public bool HandleInteractionClick(); // Return true -> stop movement
        public bool HandleInteractionHover(); // Return true -> stop movement
    }
}