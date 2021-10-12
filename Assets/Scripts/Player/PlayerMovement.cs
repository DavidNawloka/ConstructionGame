using Astutos.Saving;
using CON.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Player
{
    public class PlayerMovement : MonoBehaviour, ISaveable
    {
        [SerializeField] AudioClip[] defaultStepsSounds;

        AudioSourceManager audioLooper;
        NavMeshAgent navMeshAgent;
        Animator animator;

        static string SPEED_PARAMETER = "speed";

        private void Awake()
        {
            audioLooper = GetComponent<AudioSourceManager>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        private void Update()
        {
            animator.SetFloat(SPEED_PARAMETER, navMeshAgent.velocity.magnitude);
            HandleSound();
        }

        private void HandleSound()
        {
            if(navMeshAgent.velocity.magnitude > .2f)
            {
                audioLooper.StartLooping(defaultStepsSounds);
            }
            else
            {
                audioLooper.EndLoopingImmediate();
            }
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
