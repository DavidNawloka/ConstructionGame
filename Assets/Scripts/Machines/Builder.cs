using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Machines
{
    public class Builder : MonoBehaviour
    {
        [SerializeField] int gridWidth = 200;
        [SerializeField] int gridheight = 150;
        [SerializeField] float cellSize = 1.5f;
        [SerializeField] Vector3 gridOrigin = new Vector3(-90, 0.4f, 0);
        [SerializeField] Transform buildObjectsParent;

        BuildingGrid grid;
        bool buildMode = false;

        GameObject currentMachine;
        IPlaceable currentPlaceable;

        Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            grid = new BuildingGrid(gridWidth, gridheight, cellSize, gridOrigin);
        }

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Destroy(currentMachine);
                DeactivateBuildMode();
            }
            if (buildMode)
            {
                BuildMode();
            }
        }
        public void ActivateBuildMode(GameObject machine)
        {
            if (currentMachine != null) Destroy(currentMachine);

            buildMode = true;
            currentMachine = Instantiate(machine);
            currentPlaceable = currentMachine.GetComponent<IPlaceable>();
            currentMachine.transform.parent = buildObjectsParent;
        }
        private void BuildMode()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                int x;
                int y;
                grid.GetGridPosition(raycastHit.point, out x, out y);

                
                if (IsObstructedAll(x, y)) return;

                currentMachine.transform.position = grid.GetWorldPositionCenter(x, y);

                if (Input.GetMouseButtonDown(0))
                {
                    Placement(x, y);
                }
            }
        }

        private void Placement(int x, int y)
        {
            if (!IsPlacementPossible(x, y)) return;

            RemoveElements(currentPlaceable);

            foreach (Vector2Int takenGridPosition in currentPlaceable.GetTakenGridPositions())
            {
                grid.SetObstructed(x + takenGridPosition.x, y + takenGridPosition.y, true);
            }

            currentPlaceable.FullyPlaced();
            DeactivateBuildMode();
        }

        private bool IsPlacementPossible(int x, int y)
        {
            if (!AreEnoughElements()) return false;
            print("enough resources");

            Machine machine = currentMachine.transform.GetComponent<Machine>();
            if (machine != null && !grid.HasElement(x, y, machine.GetElementRequirement())) return false;
            print("placement possible");

            return true;
        }

        private bool IsObstructedAll(int x, int y)
        {
            bool isObstructed = false;
            foreach (Vector2Int takenGridPosition in currentPlaceable.GetTakenGridPositions())
            {
                isObstructed = grid.IsObstructed(x + takenGridPosition.x, y + takenGridPosition.y);
                if (isObstructed) return isObstructed;
            }
            return isObstructed;
        }
        
        private bool AreEnoughElements()
        {
            bool enough = true;
            foreach (InventoryItem inventoryItem in currentPlaceable.GetNeededBuildingElements())
            {
                enough = inventory.CheckItem(inventoryItem);
                if (!enough) return enough;
            }
            return enough;
        }
        private void RemoveElements(IPlaceable currentPlaceable)
        {
            foreach (InventoryItem inventoryItem in currentPlaceable.GetNeededBuildingElements())
            {
                inventory.RemoveItem(inventoryItem);
            }
        }

        private void DeactivateBuildMode()
        {
            buildMode = false;
            currentMachine = null;
            currentPlaceable = null;
        }

        
    }

}