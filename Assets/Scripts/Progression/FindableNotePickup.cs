using Astutos.Saving;
using CON.Core;
using CON.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Progression
{
    public class FindableNotePickup : MonoBehaviour,IRaycastable, ISaveable
    {
        [SerializeField] FindableNoteIdentifier findableNoteIdentifier;
        [SerializeField] float pickupClickRadius;
        [SerializeField] AudioClip[] pickupSounds;
        [SerializeField] Light pointLight;

        bool hasEquipped = false;

        AudioSourceManager audioSourceManager;
        FindableNoteManager findableNoteManager;
        private void Awake()
        {
            audioSourceManager = GetComponent<AudioSourceManager>();
            findableNoteManager = FindObjectOfType<FindableNoteManager>();
        }
        private void Update()
        {
            pointLight.intensity = .5f*Mathf.Sin(Time.time*4) + 1.5f;
        }
        private void EquipNote(bool playSound)
        {
            if(playSound) audioSourceManager.PlayOnceFromMultiple(pickupSounds);
            findableNoteManager.EquipNewNote(findableNoteIdentifier,Vector2.zero);
            hasEquipped = true;
            pointLight.gameObject.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Player" && !hasEquipped) EquipNote(true);
        }
        // Interfaces

        public CursorType GetCursorType()
        {
            return CursorType.Note;
        }

        public void HandleInteractionClick(Transform player)
        {
            EquipNote(true);
        }

        public bool InRange(Transform player)
        {
            return Vector3.Distance(player.position, transform.position) <= pickupClickRadius && !hasEquipped;
        }

        public object CaptureState()
        {
            return hasEquipped;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;
            hasEquipped = (bool)state;
            if(hasEquipped) pointLight.gameObject.SetActive(false);
        }
    }
}
