using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Core
{
    public class PlayerDetector : MonoBehaviour
    {
        public UnityEvent onPlayerEnter;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") onPlayerEnter.Invoke();
        }
    }

}