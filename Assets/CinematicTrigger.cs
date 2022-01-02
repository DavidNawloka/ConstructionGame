using Astutos.Saving;
using CON.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CON.Core
{

    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        [SerializeField] UserInterfaceManager userInterfaceManager;

        PlayableDirector playableDirector;
        bool hasPlayed = false;
        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
        public void StartCinematic()
        {
            if (hasPlayed) return;

            hasPlayed = true;
            playableDirector.Play();
            playableDirector.stopped += EndOfCinematic;
            userInterfaceManager.ActivateUI(5);
        }

        private void EndOfCinematic(PlayableDirector playableDirector)
        {
            userInterfaceManager.DeactiveUI(5);
            playableDirector.stopped -= EndOfCinematic;
        }

        public object CaptureState()
        {
            return hasPlayed;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;
            hasPlayed = (bool)state;
        }
    }

}