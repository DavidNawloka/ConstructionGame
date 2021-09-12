using CON.BuildingGrid;
using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CON.Machines
{
    public class Builder : MonoBehaviour
    {
        [SerializeField] Transform buildObjectsParent;

        public event Action<bool> onDemolishModeChange;
        public event Action<bool> onBuildModeChange;

        BuildingGridManager grid;
        BuildingGridMesh gridMesh;
        bool isPlacementMode = false;

        bool isDemolishMode = false;
        bool isBuildMode = false;

        GameObject currentMachinePrefab;
        GameObject currentMachine;
        IPlaceable currentPlaceable;
        Vector2Int[] takenGridPositions;

        Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            gridMesh = FindObjectOfType<BuildingGridMesh>();
            grid = new BuildingGridManager();


            gridMesh.InitiatePlane(grid.GetBuildingGridTexture());
            
        }
        private void Start()
        {
            onBuildModeChange(false);
        }
        private void Update()
        {
            HandleInput();
            HandleModes();
        }


        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBuildMode();
                if (isPlacementMode)
                {
                    DeactivePlacementModeDestruction();
                }
            }

            if (!isBuildMode) return;

            if (isPlacementMode && Input.GetKeyDown(KeyCode.C))
            {
                DeactivePlacementModeDestruction();
            }
            if(isPlacementMode && Input.GetKeyDown(KeyCode.V))
            {
                currentPlaceable.ChangeVersion();
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                ToggleDemolishMode();
            }
        }
        private void HandleModes()
        {
            if (isDemolishMode)
            {
                DemolishMode();
            }
            if (isPlacementMode)
            {
                HandleRotation();
                PlacementMode();
            }
        }

        public void ActivatePlacementMode(GameObject machine)
        {
            if (currentMachine != null) Destroy(currentMachine);

            isPlacementMode = true;
            currentMachinePrefab = machine;
            currentMachine = Instantiate(machine);
            currentPlaceable = currentMachine.GetComponent<IPlaceable>();
            takenGridPositions = currentPlaceable.GetTakenGridPositions();
            currentMachine.transform.parent = buildObjectsParent;
        }

        public void ToggleBuildMode()
        {
            SetActiveBuildMode(!isBuildMode);
        }
        private void SetActiveBuildMode(bool isBuildMode)
        {
            this.isBuildMode = isBuildMode;
            onBuildModeChange(isBuildMode);
            if (isDemolishMode) SetActiveDemolishMode(isBuildMode);
        }
        public void ToggleDemolishMode()
        {
            if (!isBuildMode) return;
            SetActiveDemolishMode(!isDemolishMode);
        }
        public void SetActiveDemolishMode(bool isDemolishMode)
        {
            this.isDemolishMode = isDemolishMode;
            onDemolishModeChange(isDemolishMode);
        }
        public BuildingGridMesh GetGridMesh()
        {
            return gridMesh;
        }
        
        private void DemolishMode()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit))
            {
                int x;
                int y;
                grid.GetGridPosition(raycastHit.point, out x, out y);

                if (Input.GetMouseButtonDown(0) && grid.IsObstructed(x, y))
                {
                    IPlaceable placedMachine = raycastHit.collider.GetComponent<IPlaceable>();

                    if (placedMachine == null) return;

                    Destroy(raycastHit.transform.gameObject);

                    foreach (Vector2Int takenGridPosition in placedMachine.GetTakenGridPositions())
                    {
                        grid.SetObstructed(placedMachine.GetOrigin().x + takenGridPosition.x, placedMachine.GetOrigin().y + takenGridPosition.y, false);
                    }
                    foreach (InventoryItem inventoryItem in placedMachine.GetNeededBuildingElements())
                    {
                        inventory.EquipItem(inventoryItem);
                    }
                    gridMesh.UpdateTexture(grid.GetBuildingGridTexture());

                }
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
        private void PlacementMode()
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

                if (Input.GetMouseButton(0))
                {
                    Placement(x, y);
                }
            }
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
        private void Placement(int x, int y)
        {
            if (!IsPlacementPossible(x, y)) return;

            RemoveElements(currentPlaceable);

            foreach (Vector2Int takenGridPosition in takenGridPositions)
            {
                grid.SetObstructed(x + takenGridPosition.x, y + takenGridPosition.y, true);
            }

            currentPlaceable.SetOrigin(new Vector2Int(x, y));
            currentPlaceable.FullyPlaced(this);
            DeactivatePlacementMode();
        }

        private bool IsPlacementPossible(int x, int y)
        {
            if (!AreEnoughElements()) return false;

            Machine machine = currentMachine.transform.GetComponent<Machine>();
            if (machine != null && machine.GetElementPlacementRequirement() != null &&!grid.HasElement(x, y, machine.GetElementPlacementRequirement())) return false;

            return true;
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
        private void DeactivePlacementModeDestruction()
        {
            Destroy(currentMachine);
            isPlacementMode = false;
            currentMachine = null;
            currentPlaceable = null;
        }
        private void DeactivatePlacementMode()
        {
            gridMesh.UpdateTexture(grid.GetBuildingGridTexture());
            isPlacementMode = false;
            int toRotate = (int)currentMachine.transform.localEulerAngles.y / 90;
            currentMachine = null;
            currentPlaceable = null;
            ActivatePlacementMode(currentMachinePrefab);
            for (int i = 0; i < toRotate; i++)
            {
                RotateRight();
            }
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
            currentMachine.transform.localRotation = Quaternion.Euler(new Vector3(0, currentMachine.transform.localEulerAngles.y - 90, 0));
        }
        private void RotateRight()
        {
            for (int index = 0; index < takenGridPositions.Length; index++)
            {
                int x = takenGridPositions[index].x * -1;
                int y = takenGridPositions[index].y ;
                takenGridPositions[index] = new Vector2Int(y, x);
            }
            currentMachine.transform.localRotation = Quaternion.Euler(new Vector3(0, currentMachine.transform.localEulerAngles.y + 90, 0));
        }
    }

}