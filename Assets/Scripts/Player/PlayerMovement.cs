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
        [SerializeField] FootstepSoundMapping[] footstepSoundMappings;
        [SerializeField] Terrain mainTerrain;

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
            HandleFootsteps();
        }

        private void HandleFootsteps()
        {
            if(navMeshAgent.velocity.magnitude > .2f)
            {
                audioLooper.StartLooping(GetFootstepSounds());
            }
            else
            {
                audioLooper.EndLoopingImmediate();
            }
        }

        private AudioClip[] GetFootstepSounds()
        {
            Vector2Int alphaMapPos = GetCurrentAlphaMapPosition();

            float[,,] alphaMap = mainTerrain.terrainData.GetAlphamaps(alphaMapPos.x, alphaMapPos.y, 1, 1);
            float[] textureValues = new float[mainTerrain.terrainData.terrainLayers.Length];
            textureValues[0] = alphaMap[0, 0, 0];
            textureValues[1] = alphaMap[0, 0, 1];
            textureValues[2] = alphaMap[0, 0, 2];
            textureValues[3] = alphaMap[0, 0, 3];

            foreach(FootstepSoundMapping soundMapping in footstepSoundMappings)
            {
                foreach(int index in soundMapping.terrainLayerIndices)
                {
                    if (textureValues[index] > 0)
                    {
                        return soundMapping.footstepSounds;
                    }
                }
            }

            return null;
        }

        private Vector2Int GetCurrentAlphaMapPosition()
        {
            Vector3 terrainPosition = transform.position - mainTerrain.transform.position;

            Vector3 mapPosition = new Vector3
            (terrainPosition.x / mainTerrain.terrainData.size.x, 0,
            terrainPosition.z / mainTerrain.terrainData.size.z);

            int posX = (int)(mapPosition.x * mainTerrain.terrainData.alphamapWidth);
            int posZ = (int)(mapPosition.z * mainTerrain.terrainData.alphamapHeight);

            return new Vector2Int(posX, posZ);
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

        [Serializable]
        class FootstepSoundMapping
        {
            public int[] terrainLayerIndices;
            public AudioClip[] footstepSounds;
        }
    }
}
