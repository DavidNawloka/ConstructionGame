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

        public SerializedInstruction Serialize()
        {
            return new SerializedInstruction(this.name);
        }
    }
    [System.Serializable]
    public class SerializedInstruction
    {
        public string instructionName;

        public SerializedInstruction(string instructionName)
        {
            this.instructionName = instructionName;
        }

        public Instruction DeSerialize()
        {
            return (Instruction)Resources.Load("Instructions/"+instructionName);
        }
    }

}