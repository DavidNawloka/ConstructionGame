using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core 
{
    public class AudioSourceManager : MonoBehaviour
    {
        [Tooltip("Will be taken if given AudioClip array is null")][SerializeField] AudioClip[] playedSounds;
        [SerializeField] AudioSource customAudioSource;
        [SerializeField] float loopPlayTimerMultiplier = 1;
        [SerializeField] float immediateStopTime = .1f;
        [SerializeField] bool shouldChangePitch = false;
        [SerializeField] float pitchRange = .1f;

        AudioSource audioSource;
        AudioClip[] currentAudioFilesToLoop;

        int currentAudioClipLoopIndex = 0;
        bool shouldLoop = false;
        float playTimer = Mathf.Infinity;
        float initialSpatialBlend;
        float initialVolume;

        private void Awake()
        {
            if (customAudioSource == null) audioSource = GetComponent<AudioSource>();
            else audioSource = customAudioSource;

            initialSpatialBlend = audioSource.spatialBlend;
            initialVolume = audioSource.volume;
        }
        private void Update()
        {
            if (!shouldLoop) return;

            playTimer += Time.deltaTime;
            if (playTimer <= currentAudioFilesToLoop[currentAudioClipLoopIndex].length * loopPlayTimerMultiplier) return;

            playTimer = 0;
            if (shouldChangePitch) ChangePitch();
            audioSource.PlayOneShot(currentAudioFilesToLoop[currentAudioClipLoopIndex]);
            currentAudioClipLoopIndex++;
            if (currentAudioClipLoopIndex == currentAudioFilesToLoop.Length) currentAudioClipLoopIndex = GetRandomIndex(currentAudioFilesToLoop);
        }
        public void PlayOnceFromMultipleAdjust(AudioClip[] audioClipList,float spatialBlend, float volume)
        {
            audioSource.spatialBlend = spatialBlend;
            audioSource.volume = volume;
            PlayOnce(audioClipList[GetRandomIndex(audioClipList)]);
        }

        public void PlayOnceFromMultiple(AudioClip[] audioClipList)
        {
            PlayOnce(audioClipList[GetRandomIndex(audioClipList)]);
        }

        public void PlayOnce(AudioClip audioClip = null)
        {
            if (shouldChangePitch) ChangePitch();

            if (audioClip == null) audioSource.PlayOneShot(playedSounds[GetRandomIndex(playedSounds)]);
            else audioSource.PlayOneShot(audioClip);
        }
        public void ToggleLooping(AudioClip[] audioFilesToLoop = null)
        {
            SetActiveLooping(!shouldLoop, audioFilesToLoop);
        }
        public void StartLooping(AudioClip[] audioFilesToLoop)
        {
            SetActiveLooping(true, audioFilesToLoop);
        }
        public bool IsLooping()
        {
            return shouldLoop;
        }
        public void EndLoopingImmediate()
        {
            SetActiveLooping(false,null);
            audioSource.Stop();
        }
        public void EndLoopingSmooth()
        {
            SetActiveLooping(false, null);
            StartCoroutine(StopPlayingImmediate());
        }

        public void UpdateAudioFilesToLoop(AudioClip[] newAudioFilesToLoop)
        {
            currentAudioFilesToLoop = newAudioFilesToLoop;
        }

        private void ChangePitch()
        {
            audioSource.pitch = Random.Range(1 - pitchRange / 2, 1 + pitchRange / 2);
        }

        private void SetActiveLooping(bool isPlaying, AudioClip[] audioFilesToLoop)
        {
            currentAudioFilesToLoop = audioFilesToLoop;
            shouldLoop = isPlaying;
            if (isPlaying) currentAudioClipLoopIndex = GetRandomIndex(currentAudioFilesToLoop);
        }
        private int GetRandomIndex(object[] array)
        {
            return Random.Range(0, array.Length);
        }
        private IEnumerator StopPlayingImmediate()
        {
            float timer = immediateStopTime;
            float defaultVolume = audioSource.volume;
            while (timer > 0)
            {
                audioSource.volume = (timer / immediateStopTime) * defaultVolume;
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = defaultVolume;
        }

        public void ResetAudioSourceParameters()
        {
            audioSource.spatialBlend = initialSpatialBlend;
            audioSource.volume = initialVolume;
        }

    }
}