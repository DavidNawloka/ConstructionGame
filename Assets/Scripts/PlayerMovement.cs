using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        NavMeshAgent navMeshAgent;
        Animator animator;

        static string SPEED_PARAMETER = "speed";

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            
            if (Input.GetMouseButton(0))
            {
                MovementInteraction();
            }
            animator.SetFloat(SPEED_PARAMETER, navMeshAgent.velocity.magnitude);
        }

        private bool MovementInteraction()
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            
            
            if (Physics.Raycast(cameraRay, out raycastHit))
            {

                NavMeshPath navMeshPath = new NavMeshPath();
                navMeshAgent.CalculatePath(raycastHit.point, navMeshPath);

                if(navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    navMeshAgent.SetPath(navMeshPath);
                    return true;
                }
            }

            print("Path Invalid!");
            return false;
        }
    }

}