using Astutos.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Player
{
    public class PlayerMovement : MonoBehaviour, ISaveable
    {
        NavMeshAgent navMeshAgent;
        Animator animator;

        static string SPEED_PARAMETER = "speed";

        private void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        private void Update()
        {
            animator.SetFloat(SPEED_PARAMETER, navMeshAgent.velocity.magnitude);
        }

        public bool MoveTo(Ray cameraRay)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(cameraRay, out raycastHit))
            {

                NavMeshPath navMeshPath = new NavMeshPath();
                navMeshAgent.CalculatePath(raycastHit.point, navMeshPath);

                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    navMeshAgent.SetPath(navMeshPath);
                    return true;
                }
            }

            print("Path Invalid!");
            return false;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            navMeshAgent.Warp(((SerializableVector3)state).ToVector());
        }
    }
}
