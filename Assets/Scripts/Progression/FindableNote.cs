using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Progression
{
    [CreateAssetMenu(fileName = "findeableNote", menuName = "Progression/Create new findable Note")]
    public class FindableNote : ScriptableObject
    {
        public string text;
        public Sprite background;
    }

}