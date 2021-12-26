using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Progression
{
    public class FindableNoteManager : MonoBehaviour
    {
        [SerializeField] Transform findableNotesParent;
        public void EquipNewNote(FindableNote newFindeableNote)
        {
            FindableNote instantiatedNote = Instantiate<FindableNote>(newFindeableNote, findableNotesParent);
        }
    }

}