using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core 
{
    public class AudioLooper : MonoBehaviour
    {
        [SerializeField] AudioClip[] audioFilesToLoop;
        [SerializeField] float playTimerMultiplier = 1;

        AudioSource audioSource;

        int currentAudioClipIndex = 0;
        bool shouldPlay = false;
        float playTimer = Mathf.Infinity;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        private void Start()
        {
            currentAudioClipIndex = GetRandomIndex();
        }
        private void Update()
        {
            playTimer += Time.deltaTime;
            if (!shouldPlay || playTimer <= audioFilesToLoop[currentAudioClipIndex].length * playTimerMultiplier) return;

            playTimer = 0;
            audioSource.PlayOneShot(audioFilesToLoop[currentAudioClipIndex]);
            currentAudioClipIndex++;
            if (currentAudioClipIndex == audioFilesToLoop.Length) currentAudioClipIndex = GetRandomIndex();
        }
        private int GetRandomIndex()
        {
            return Random.Range(0, audioFilesToLoop.Length);
        }
        public void TogglePlaying()
        {
            shouldPlay = !shouldPlay;
        }
        public void StartPlaying()
        {
            shouldPlay = true;
        }
        public void EndPlaying()
        {
            shouldPlay = false;
        }

    }
}