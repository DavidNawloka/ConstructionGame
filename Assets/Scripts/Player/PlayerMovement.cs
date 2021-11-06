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
        [SerializeField] Terrain terrainBelow;
        [SerializeField] float minVelocityForSound = .2f;

        Dictionary<int, AudioClip[]> footstepSoundsLookup = new Dictionary<int, AudioClip[]>();

        AudioSourceManager audioSourceManager;
        NavMeshAgent navMeshAgent;
        Animator animator;
        Terrain currentTerrain;

        static string SPEED_PARAMETER = "speed";

        private void Awake()
        {
            audioSourceManager = GetComponent<AudioSourceManager>();
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            currentTerrain = mainTerrain;

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
            Vector2Int alphaMapPos = GetCurrentAlphaMapPosition(true);

            float[,,] alphaMap = currentTerrain.terrainData.GetAlphamaps(alphaMapPos.x, alphaMapPos.y, 1, 1);
            float[] textureValues = new float[currentTerrain.terrainData.terrainLayers.Length];

            int biggestIndex = 0;
            for (int layerIndex = 0; layerIndex < textureValues.Length; layerIndex++)
            {
                textureValues[layerIndex] = alphaMap[0, 0, layerIndex];

                if (textureValues[biggestIndex] < textureValues[layerIndex]) biggestIndex = layerIndex;
            }

            return footstepSoundsLookup[biggestIndex];
        }

        private Vector2Int GetCurrentAlphaMapPosition(bool shouldCheckTerrain)
        {
            Vector3 terrainPosition = transform.position - currentTerrain.transform.position;

            Vector3 mapPosition = new Vector3
            (terrainPosition.x / currentTerrain.terrainData.size.x, 0,
            terrainPosition.z / currentTerrain.terrainData.size.z);

            int posX = (int)(mapPosition.x * currentTerrain.terrainData.alphamapWidth);
            int posZ = (int)(mapPosition.z * currentTerrain.terrainData.alphamapHeight);

            if (shouldCheckTerrain && posZ < 0 && currentTerrain == mainTerrain) // TODO: Merge all terrains into 1
            {
                currentTerrain = terrainBelow;
                return GetCurrentAlphaMapPosition(false);
            }
            else if (shouldCheckTerrain && posZ >= currentTerrain.terrainData.alphamapHeight && currentTerrain == terrainBelow)
            {
                currentTerrain = mainTerrain;
                return GetCurrentAlphaMapPosition(false);
            }

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
