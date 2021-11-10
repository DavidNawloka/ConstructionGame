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
        [SerializeField] float pitchRange = 10f;

        AudioSource audioSource;
        AudioClip[] currentAudioFilesToLoop;

        int currentAudioClipLoopIndex = 0;
        bool shouldPlay = false;
        float playTimer = Mathf.Infinity;

        private void Awake()
        {
            if (customAudioSource == null) audioSource = GetComponent<AudioSource>();
            else audioSource = customAudioSource;
        }
        private void Update()
        {
            if (!shouldPlay) return;

            playTimer += Time.deltaTime;
            if (playTimer <= currentAudioFilesToLoop[currentAudioClipLoopIndex].length * loopPlayTimerMultiplier) return;

            playTimer = 0;
            if (shouldChangePitch) ChangePitch();
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
            if (shouldChangePitch) ChangePitch();
            if (audioClip == null) audioSource.PlayOneShot(playedSounds[GetRandomIndex(playedSounds)]);
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

        private void ChangePitch()
        {
            audioSource.pitch = Random.Range(1 - pitchRange / 2, 1 + pitchRange / 2);
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