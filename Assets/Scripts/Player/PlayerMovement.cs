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
        [SerializeField] float minVelocityForSound = .2f;

        Dictionary<int, AudioClip[]> footstepSoundsLookup = new Dictionary<int, AudioClip[]>();

        AudioSourceManager audioSourceManager;
        NavMeshAgent navMeshAgent;
        Animator animator;

        static string SPEED_PARAMETER = "speed";

        private void Awake()
        {
            audioSourceManager = GetComponent<AudioSourceManager>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            BuildFootstepSoundsLookupDictionary();
        }

        private void Update()
        {
            animator.SetFloat(SPEED_PARAMETER, navMeshAgent.velocity.magnitude);
            HandleFootsteps();
        }

        private void HandleFootsteps()
        {
            if(navMeshAgent.velocity.magnitude > minVelocityForSound)
            {
                audioSourceManager.StartLooping(GetFootstepSounds());
            }
            else
            {
                audioSourceManager.EndLoopingImmediate();
            }
        }

        private AudioClip[] GetFootstepSounds()
        {
            Vector2Int alphaMapPos = GetCurrentAlphaMapPosition();

            float[,,] alphaMap = mainTerrain.terrainData.GetAlphamaps(alphaMapPos.x, alphaMapPos.y, 1, 1);
            float[] textureValues = new float[mainTerrain.terrainData.terrainLayers.Length];

            int biggestIndex = 0;
            for (int layerIndex = 0; layerIndex < textureValues.Length; layerIndex++)
            {
                textureValues[layerIndex] = alphaMap[0, 0, layerIndex];

                if (textureValues[biggestIndex] < textureValues[layerIndex]) biggestIndex = layerIndex;
            }

            return footstepSoundsLookup[biggestIndex];
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

        public bool MoveTo(RaycastHit raycastHit)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
            navMeshAgent.CalculatePath(raycastHit.point, navMeshPath);

            if (navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                navMeshAgent.SetPath(navMeshPath);
                return true;
            }

            return false;
        }


        private void BuildFootstepSoundsLookupDictionary()
        {
            for (int layerIndexTerrain = 0; layerIndexTerrain < mainTerrain.terrainData.terrainLayers.Length; layerIndexTerrain++)
            {
                foreach (FootstepSoundMapping soundMapping in footstepSoundMappings)
                {
                    if (isCorrectSoundMapping(soundMapping, layerIndexTerrain)) break;
                }
            }

            
        }

        private bool isCorrectSoundMapping(FootstepSoundMapping soundMapping, int layerIndexTerrain)
        {
            foreach (int layerIndexSoundMapping in soundMapping.terrainLayerIndices)
            {
                if (layerIndexSoundMapping == layerIndexTerrain)
                {
                    footstepSoundsLookup[layerIndexTerrain] = soundMapping.footstepSounds;
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
