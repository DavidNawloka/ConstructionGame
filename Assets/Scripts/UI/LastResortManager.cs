using CON.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace CON.UI
{
    public class LastResortManager : MonoBehaviour, IRaycastable
    {
        [SerializeField] MoveableWindow moveableWindow;
        [SerializeField] Transform moveableWindowConnect;
        [SerializeField] Animation animationComponent;


        bool isUnlocked = false;

        public void OnEquipKeyButtonPressed()
        {
            if (isUnlocked) return;
            isUnlocked = true;
            animationComponent.Play();
            GetComponent<PlayableDirector>().Play();
            moveableWindow.SetActiveCanvas(false, moveableWindowConnect);
        }

        public CursorType GetCursorType()
        {
            return CursorType.Placeable;
        }

        public bool InRange(Transform player)
        {
            return true;
        }

        public void HandleInteractionClick(Transform player)
        {
            moveableWindow.ToggleCanvas(moveableWindowConnect);
        }
        public object CaptureState()
        {
            return isUnlocked;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;
            isUnlocked = (bool)state;
            if (isUnlocked)
            {
                animationComponent[animationComponent.clip.name].time = animationComponent[animationComponent.clip.name].length;
                animationComponent.Play();
            }
        }
    }

}