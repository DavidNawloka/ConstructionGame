using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core 
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceManager : MonoBehaviour
    {
        [Tooltip("Will be taken if given AudioClip array is null")][SerializeField] AudioClip[] playedSounds;
        [SerializeField] float loopPlayTimerMultiplier = 1;
        [SerializeField] float immediateStopTime = .1f;

        AudioSource audioSource;
        AudioClip[] currentAudioFilesToLoop;

        int currentAudioClipLoopIndex = 0;
        bool shouldPlay = false;
        float playTimer = Mathf.Infinity;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        private void Update()
        {
            playTimer += Time.deltaTime;
            if (!shouldPlay || playTimer <= currentAudioFilesToLoop[currentAudioClipLoopIndex].length * loopPlayTimerMultiplier) return;

            playTimer = 0;
            audioSource.PlayOneShot(currentAudioFilesToLoop[currentAudioClipLoopIndex]);
            currentAudioClipLoopIndex++;
            if (currentAudioClipLoopIndex == currentAudioFilesToLoop.Length) currentAudioClipLoopIndex = GetRandomIndex(currentAudioFilesToLoop);
        }
        
        public void PlayOnceFromMultiple(AudioClip[] audioClipList)
        {
            PlayOnce(audioClipList[GetRandomIndex(audioClipList)]);
        }

        public void PlayOnce(AudioClip audioClip = null)
        {
            if(audioClip == null) audioSource.PlayOneShot(playedSounds[GetRandomIndex(playedSounds)]);
            else audioSource.PlayOneShot(audioClip);
        }
        public void ToggleLooping(AudioClip[] audioFilesToLoop = null)
        {
            SetActiveLooping(!shouldPlay, audioFilesToLoop);
        }
        public void StartLooping(AudioClip[] audioFilesToLoop)
        {
            SetActiveLooping(true, audioFilesToLoop);
        }
        public void EndLooping()
        {
            SetActiveLooping(false,null);
        }
        public void EndLoopingImmediate()
        {
            SetActiveLooping(false, null);
            StartCoroutine(StopPlayingImmediate());
        }

        public void UpdateAudioFilesToLoop(AudioClip[] newAudioFilesToLoop)
        {
            currentAudioFilesToLoop = newAudioFilesToLoop;
        }

        private void SetActiveLooping(bool isPlaying, AudioClip[] audioFilesToLoop)
        {
            currentAudioFilesToLoop = audioFilesToLoop;
            shouldPlay = isPlaying;
            if(isPlaying) currentAudioClipLoopIndex = GetRandomIndex(currentAudioFilesToLoop);
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
                timer += Time.deltaTime;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = defaultVolume;
        }

    }
}