using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    public class LookAtCamera : MonoBehaviour
    {
        Transform followCamera;
        private void Awake()
        {
            followCamera = GameObject.FindGameObjectWithTag("FollowCamera").transform;
        }
        private void Update()
        {
            transform.LookAt(followCamera.position);
        }
    }
}
