using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicManager : MonoBehaviour
    {
        [SerializeField] AudioClip[] suspensfulBackgroundTracks;
        [SerializeField] AudioClip[] relaxingBackgroundTracks;
        [SerializeField] bool shouldDelayInitialStart = false;
        [SerializeField] float startingDelay = 5f;
        [SerializeField] bool shouldSwitchBackgroundTracks = false;
        [SerializeField] int startingTrackIndex = 0;
        [SerializeField] float timeBetweenTracks = 12;
        [SerializeField] float timeVariationFactor = 0.1f;
        [SerializeField] float timeToStop = .5f;

        AudioSource audioSource;

        int currentBackgroundMusicClipIndex;
        AudioClip[] currentBackgroundTracks;

        private void Awake()
        {
            currentBackgroundMusicClipIndex = startingTrackIndex;
            currentBackgroundTracks = relaxingBackgroundTracks;
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(LoopMusic());
        }
        public IEnumerator MuteMusic()
        {
            float timer = timeToStop;
            float defaultVolume = audioSource.volume;
            while (timer > 0)
            {
                audioSource.volume = (timer / timeToStop) * defaultVolume;
                timer -= Time.unscaledDeltaTime;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = defaultVolume;
        }
        private IEnumerator LoopMusic()
        {
            if (shouldDelayInitialStart) yield return new WaitForSecondsRealtime(startingDelay);
            while (true)
            {
                audioSource.PlayOneShot(currentBackgroundTracks[currentBackgroundMusicClipIndex]);
                yield return new WaitForSecondsRealtime(currentBackgroundTracks[currentBackgroundMusicClipIndex].length);

                SwitchCurrentTracks();

                currentBackgroundMusicClipIndex = GetRandomIndex(currentBackgroundTracks);

                yield return new WaitForSecondsRealtime(GetTimeBetweenTracks());
            }

        }

        private void SwitchCurrentTracks()
        {
            if (!shouldSwitchBackgroundTracks) return;
            if (currentBackgroundTracks == suspensfulBackgroundTracks) currentBackgroundTracks = relaxingBackgroundTracks;
            else currentBackgroundTracks = suspensfulBackgroundTracks;
        }

        private float GetTimeBetweenTracks()
        {
            return Random.Range(timeBetweenTracks - timeBetweenTracks * timeVariationFactor, timeBetweenTracks + timeBetweenTracks * timeVariationFactor);
        }

        private int GetRandomIndex(object[] array)
        {
            return Random.Range(0, array.Length);
        }
    }

}