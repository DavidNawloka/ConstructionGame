using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField] float minX;
        [SerializeField] float maxX;
        [SerializeField] float minZ;
        [SerializeField] float maxZ;

        Transform player;

        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        private void LateUpdate()
        {
            transform.position = new Vector3(
                Mathf.Clamp(player.position.x, minX, maxX),
                transform.position.y,
                Mathf.Clamp(player.position.z, minZ, maxZ));
        }
    }

}