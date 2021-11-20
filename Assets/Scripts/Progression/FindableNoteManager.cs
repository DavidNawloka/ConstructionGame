using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Progression
{
    public class FindableNoteManager : MonoBehaviour
    {
        [SerializeField] Transform findableNotesParent;
        [SerializeField] FindableNoteVisualiser findableNoteVisualiserPrefab;
        public void EquipNewNote(FindableNote newFindeableNote)
        {
            FindableNoteVisualiser instantiatedNote = Instantiate<FindableNoteVisualiser>(findableNoteVisualiserPrefab, findableNotesParent);
            instantiatedNote.InitialiseVisualisation(newFindeableNote);
        }
    }

}