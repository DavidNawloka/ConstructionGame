using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CON.Progression
{
    [CreateAssetMenu(fileName = "Unlockable", menuName = "Progression/Create New Unlockable")]
    public class Unlockable : ScriptableObject
    {
        public GameObject prefab;
        public Sprite sprite;
        public new string name;
        public string description;
        public InventoryItem[] elementRequirements;
        public PlaceableType type;

    }

    public enum PlaceableType
    {
        Conveyor,
        Gatherer,
        Crafter
    }

}