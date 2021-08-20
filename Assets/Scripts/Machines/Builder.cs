using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        BuildingGridMesh gridMesh;
        bool buildMode = false;

        GameObject currentMachine;
        IPlaceable currentPlaceable;
        Vector2Int[] takenGridPositions;

        Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            gridMesh = FindObjectOfType<BuildingGridMesh>();
            grid = new BuildingGrid(gridWidth, gridheight, cellSize, gridOrigin);
            gridMesh.BuildPlane(gridWidth, gridheight, grid.GetBuildingGridTexture());
        }

        public BuildingGrid GetGrid()
        {
            return grid;
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
                HandleRotation();
                BuildMode();
            }
        }

        private void HandleRotation()
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.mouseScrollDelta.y >= 1f)
            {
                RotateLeft();
            }
            if (Input.GetKeyDown(KeyCode.E) || Input.mouseScrollDelta.y <= -1f)
            {
                RotateRight();
            }
        }

        public void ActivateBuildMode(GameObject machine)
        {
            if (currentMachine != null) Destroy(currentMachine);

            buildMode = true;
            currentMachine = Instantiate(machine);
            currentPlaceable = currentMachine.GetComponent<IPlaceable>();
            takenGridPositions = currentPlaceable.GetTakenGridPositions();
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

            foreach (Vector2Int takenGridPosition in takenGridPositions)
            {
                grid.SetObstructed(x + takenGridPosition.x, y + takenGridPosition.y, true);
            }

            currentPlaceable.FullyPlaced();
            DeactivateBuildMode();
        }

        private bool IsPlacementPossible(int x, int y)
        {
            if (!AreEnoughElements()) return false;

            Machine machine = currentMachine.transform.GetComponent<Machine>();
            if (machine != null && !grid.HasElement(x, y, machine.GetElementPlacementRequirement())) return false;

            return true;
        }

        private bool IsObstructedAll(int x, int y)
        {
            bool isObstructed = false;
            foreach (Vector2Int takenGridPosition in takenGridPositions)
            {
                isObstructed = grid.IsObstructed(x + takenGridPosition.x, y + takenGridPosition.y);
                if (isObstructed) return isObstructed;
            }
            return isObstructed;
        }
        // TODO: Refactor Rotation
        private void RotateLeft()
        {
            for (int index = 0; index < takenGridPositions.Length; index++)
            {
                int x = takenGridPositions[index].x;
                int y = takenGridPositions[index].y * -1;
                takenGridPositions[index] = new Vector2Int(y, x);
            }
            currentMachine.transform.rotation = Quaternion.Euler(new Vector3(0, currentMachine.transform.eulerAngles.y - 90, 0));
        }
        private void RotateRight()
        {
            for (int index = 0; index < takenGridPositions.Length; index++)
            {
                int x = takenGridPositions[index].x * -1;
                int y = takenGridPositions[index].y ;
                takenGridPositions[index] = new Vector2Int(y, x);
            }
            currentMachine.transform.rotation = Quaternion.Euler(new Vector3(0, currentMachine.transform.eulerAngles.y + 90, 0));
        }
        private bool AreEnoughElements()
        {
            bool enough = true;
            foreach (InventoryItem inventoryItem in currentPlaceable.GetNeededBuildingElements())
            {
                enough = inventory.HasItem(inventoryItem);
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
            gridMesh.UpdateTexture(grid.GetBuildingGridTexture());
            buildMode = false;
            currentMachine = null;
            currentPlaceable = null;
        }

        
    }

}