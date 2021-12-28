using Astutos.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Progression
{
    public class FindableNoteManager : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform findableNotesParent;
        [SerializeField] int noteAmountForKey = 8;
        [SerializeField] FindableNote[] allFindableNotes;
        [Header("Last Resort Related")]
        [SerializeField] CanvasGroup equipKeyCanvasGroup;

        List<FindableNote> foundNotes = new List<FindableNote>();

        bool shouldCheckKeyDistance = false;

        private void Update()
        {
            if (foundNotes.Count != noteAmountForKey || !shouldCheckKeyDistance) return;

            CheckIfKeyPlacementCorrect();
        }
        public void EquipNewNote(FindableNoteIdentifier findableNoteIdentifier, Vector2 anchoredPosition)
        {
            FindableNote instantiatedNote = null;
            foreach (FindableNote savedFindableNote in allFindableNotes)
            {
                if(savedFindableNote.noteType == findableNoteIdentifier)
                {
                    instantiatedNote = Instantiate<FindableNote>(savedFindableNote, findableNotesParent);
                    break;
                }
            }
            
            foundNotes.Add(instantiatedNote);
            instantiatedNote.OnMovementStatusChange.AddListener(OnFindableNoteMovingStatusChange);
            instantiatedNote.GetRectTransform().anchoredPosition = anchoredPosition;
        }

        private void CheckIfKeyPlacementCorrect()
        {
            if (foundNotes.Count != noteAmountForKey) return;
            foreach(FindableNote foundNote in foundNotes)
            {
                if (!foundNote.IsKeyPlacement()) return;
            }

            UnlockedKey();
        }

        private void UnlockedKey()
        {
            foreach (FindableNote foundNote in foundNotes)
            {
                foundNote.KeyPlacementFound();
            }

            FindableNote[] foundNotesArray = foundNotes.ToArray();

            Array.Sort(foundNotesArray, null, new FoundNotesSorter());

            GetToPerfectPosition(foundNotesArray, 1, 0);
            GetToPerfectPosition(foundNotesArray, 2, 1);
            GetToPerfectPosition(foundNotesArray, 3, 0);
            GetToPerfectPosition(foundNotesArray, 5, 3);
            GetToPerfectPosition(foundNotesArray, 4, 5);
            GetToPerfectPosition(foundNotesArray, 6, 4);
            GetToPerfectPosition(foundNotesArray, 7, 6);

            equipKeyCanvasGroup.alpha = 1;
            equipKeyCanvasGroup.interactable = true;
        }
        private void GetToPerfectPosition(FindableNote[] foundNotesArray, int noteIndex, int referenceNoteIndex)
        {
            foundNotesArray[noteIndex].GetComponent<RectTransform>().anchoredPosition = foundNotesArray[referenceNoteIndex].GetRectTransform().anchoredPosition + -1 * foundNotesArray[noteIndex].GetPerfectDistanceTo(referenceNoteIndex);
        }

        private void OnFindableNoteMovingStatusChange(bool isMoving)
        {
            shouldCheckKeyDistance = isMoving;
        }

        private int GetFindableNoteIndex(FindableNoteIdentifier findableNoteIdentifier)
        {
            for (int i = 0; i < allFindableNotes.Length; i++)
            {
                if (findableNoteIdentifier == allFindableNotes[i].noteType) return i;
            }
            return -1;
        }

        public object CaptureState()
        {
            SerializableFoundNote[] noteIndices = new SerializableFoundNote[foundNotes.Count];

            for (int i = 0; i < foundNotes.Count; i++)
            {
                noteIndices[i] = new SerializableFoundNote(GetFindableNoteIndex(foundNotes[i].noteType),foundNotes[i].GetRectTransform().anchoredPosition);
            }
            return noteIndices;
        }

        public void RestoreState(object state)
        {
            if (state == null) return;
            SerializableFoundNote[] loadedNotes = (SerializableFoundNote[])state;
            foreach (SerializableFoundNote loadedNote in loadedNotes)
            {
                EquipNewNote((FindableNoteIdentifier)loadedNote.findableNoteIndex,loadedNote.anchoredPosition.ToVector());
            }
            CheckIfKeyPlacementCorrect();
        }

        class FoundNotesSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                FindableNote note1 = (FindableNote)x;
                FindableNote note2 = (FindableNote)y;

                if ((int)note1.noteType< (int)note2.noteType) return -1;
                else if ((int)note1.noteType > (int)note2.noteType) return 1;
                else return 0;
            }
        }

        [System.Serializable]
        class SerializableFoundNote
        {
            public int findableNoteIndex;
            public SerializableVector2 anchoredPosition;

            public SerializableFoundNote(int findableNoteIndex, Vector2 anchoredPosition)
            {
                this.findableNoteIndex = findableNoteIndex;
                this.anchoredPosition = new SerializableVector2(anchoredPosition);
            }
        }

        
    }

}