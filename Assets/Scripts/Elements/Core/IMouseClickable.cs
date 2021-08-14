using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    interface IMouseClickable
    {
        public bool HandleInteractionClick(Transform player); // Return true -> stop movement
    }
}