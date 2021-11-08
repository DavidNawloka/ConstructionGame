using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    interface IRaycastable
    {
        public CursorType GetCursorType();
        public bool InRange(Transform player);
        public void HandleInteractionClick(Transform player); // Return true -> stop movement
    }
}