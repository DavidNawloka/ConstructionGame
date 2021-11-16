using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Player
{
    interface IRaycastable
    {
        public CursorType GetCursorType();
        public bool InRange(Transform player);
        public void HandleInteractionClick(Transform player); // Return true -> stop movement
    }
}