using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CON.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        [SerializeField] List<Unlockable> startingPlaceables;

        List<Unlockable> unlockedPlaceables = new List<Unlockable>();

        [HideInInspector] public UnityEvent<Unlockable> OnPlaceableUnlocked;
        [HideInInspector] public UnityEvent<Inventory> OnMachineProducedElement;

        Inventory inventory;
        private void Awake()
        {
            inventory = GetComponent<Inventory>();
        }

        private void Start()
        {
            OnMachineProducedElement.Invoke(inventory);
            foreach (Unlockable placeable in startingPlaceables)
            {
                UnlockPlaceable(placeable);
            }
        }

        public Inventory GetInventory()
        {
            return inventory;
        }

        public void InventoryChange(Inventory inventory)
        {
            OnMachineProducedElement.Invoke(inventory);
        }

        public void UnlockPlaceable(Unlockable unlockable)
        {
            unlockedPlaceables.Add(unlockable);
            OnPlaceableUnlocked.Invoke(unlockable);
        }



#if UNITY_EDITOR
        public void InstantiateConnectors()
        {
            DeleteConnectors();
            foreach (Transform node in transform)
            {
                node.GetComponent<ProgressionNode>().InstantiateConnectors();
                node.GetComponent<ProgressionNode>().UpdateUnlockableVisualisation();
            }
        }


        public void DeleteConnectors()
        {
            GameObject[] gameObjectsToDestroy = new GameObject[transform.parent.childCount];
            for (int childIndex = 0; childIndex < transform.parent.childCount; childIndex++)
            {
                ProgressionManager progressionManager = transform.parent.GetChild(childIndex).GetComponent<ProgressionManager>();
                if (progressionManager != null) continue;
                gameObjectsToDestroy[childIndex] = transform.parent.GetChild(childIndex).gameObject;
            }
            foreach(GameObject nodeConnector in gameObjectsToDestroy)
            {
                DestroyImmediate(nodeConnector);
            }
        }
#endif
    }

}