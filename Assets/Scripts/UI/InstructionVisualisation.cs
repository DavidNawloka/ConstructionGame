using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CON.UI
{
    public class InstructionVisualisation: MonoBehaviour
    {
        [SerializeField] Image[] requirements;
        [SerializeField] Image outcome;


        public void UpdateInstruction(Instruction instruction)
        {
            for (int requirementIndex = 0; requirementIndex < requirements.Length; requirementIndex++)
            {
                if (requirementIndex >= instruction.requirements.Length)
                {
                    requirements[requirementIndex].gameObject.SetActive(false);
                    continue;
                }
                requirements[requirementIndex].sprite = instruction.requirements[requirementIndex].element.sprite;
            }
            outcome.sprite = instruction.outcome.element.sprite;
        }
    }

}