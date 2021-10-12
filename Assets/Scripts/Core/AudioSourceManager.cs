using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core 
{
    public class AudioSourceManager : MonoBehaviour
    {
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
            if (currentAudioClipLoopIndex == currentAudioFilesToLoop.Length) currentAudioClipLoopIndex = GetRandomIndex();
        }
        private int GetRandomIndex()
        {
            return Random.Range(0, currentAudioFilesToLoop.Length);
        }
        public void PlayOnce(AudioClip audioClip)
        {
            audioSource.PlayOneShot(audioClip);
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
        private void SetActiveLooping(bool isPlaying, AudioClip[] audioFilesToLoop)
        {
            currentAudioFilesToLoop = audioFilesToLoop;
            shouldPlay = isPlaying;
            if(isPlaying) currentAudioClipLoopIndex = GetRandomIndex();
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