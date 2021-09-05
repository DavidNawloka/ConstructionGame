using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Elements
{
    [CreateAssetMenu(fileName = "Instruction", menuName = "Elements/Create New Instruction")]
    public class Instruction : ScriptableObject
    {
        public InventoryItem[] requirements;
        public InventoryItem outcome;
    }

}