using Astutos.Saving;
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
    public class Builder : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform buildObjectsParent;
        [SerializeField] GameObject[] placeableObjectsPrefabs;

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
        Dictionary<SerializableVector3,SavedPlaceable> builtObjects = new Dictionary<SerializableVector3, SavedPlaceable>();

        Inventory inventory;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            gridMesh = FindObjectOfType<BuildingGridMesh>();
        }

        private void Start()
        {
            if (grid == null)
            {
                Texture2D texture = GetDefault2DTexture();
                grid = new BuildingGridManager(BuildingGridAssetManager.GetGrid(), texture);
                gridMesh.InitiatePlane(texture);
            }
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
            if(isDemolishMode) DeactivePlacementModeDestruction();
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

                    //TODO: Optimization when there are a lot of keyvaluepairs to go through
                    foreach(KeyValuePair<SerializableVector3,SavedPlaceable> keyValuePair in builtObjects)
                    {
                        if (Vector3.Distance(keyValuePair.Key.ToVector(), raycastHit.collider.transform.position) < 1f)
                        {
                            print(builtObjects.Remove(keyValuePair.Key));
                            break;
                        }
                    }
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
            builtObjects.Add(new SerializableVector3(currentMachine.transform.position),new SavedPlaceable(GetPlaceableObjectsID(currentMachinePrefab), currentMachine.transform.eulerAngles, new Vector2Int(x,y),takenGridPositions, currentPlaceable));
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

        private int GetPlaceableObjectsID(GameObject placeablePrefab)
        {
            for (int id = 0; id < placeableObjectsPrefabs.Length; id++)
            {
                if (placeableObjectsPrefabs[id] == placeablePrefab) return id;
            }
            return -1;
        }

        private Texture2D GetDefault2DTexture()
        {
            BuildingGridSettings gridSettings = BuildingGridAssetManager.LoadSettings();
            Texture2D texture = new Texture2D(gridSettings.width, gridSettings.height, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;

            Graphics.CopyTexture(BuildingGridAssetManager.GetGridTexture(), texture);
            return texture;
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

        // Interface Implementations


        // TODO: Save spawned elements by scanning whole scene for them and saving the necessary information
        public object CaptureState()
        {
            BuildingGridSettings gridSettings = BuildingGridAssetManager.LoadSettings();
            bool[,] obstructedList = new bool[gridSettings.width, gridSettings.height];

            for (int x = 0; x < gridSettings.width; x++)
            {
                for (int y = 0; y < gridSettings.height; y++)
                {
                    obstructedList[x, y] = grid.gridArray[x, y].obstructed;
                }
            }

            Dictionary<SerializableVector3, SavedPlaceable> newBuiltObjects = new Dictionary<SerializableVector3, SavedPlaceable>();

            foreach(KeyValuePair<SerializableVector3,SavedPlaceable> keyValuePair in builtObjects)
            {
                newBuiltObjects.Add(keyValuePair.Key, new SavedPlaceable(keyValuePair.Value.id, keyValuePair.Value.eulerRotation.ToVector(), keyValuePair.Value.origin.ToVector(), keyValuePair.Value.GetTakenGridPositions(), ((IPlaceable)keyValuePair.Value.variableInformation).GetInformationToSave()));
            }

            return new SaveData(obstructedList, newBuiltObjects);
        }

        public void RestoreState(object state)
        {
            SaveData saveData = (SaveData)state;

            BuildingGridSettings gridSettings = BuildingGridAssetManager.LoadSettings();
            GridCell[,] gridArray = BuildingGridAssetManager.GetGrid();

            Texture2D gridTexture = GetDefault2DTexture();

            bool[,] obstructedList = saveData.obstructedList;

            for (int x = 0; x < gridSettings.width; x++)
            {
                for (int y = 0; y < gridSettings.height; y++)
                {
                    gridArray[x, y].obstructed = obstructedList[x, y];
                    if (obstructedList[x, y]) gridTexture.SetPixel(x, y, Color.red);
                }
            }
            gridTexture.Apply();

            gridMesh = FindObjectOfType<BuildingGridMesh>();
            gridMesh.InitiatePlane(gridTexture);
            grid = new BuildingGridManager(gridArray,gridTexture);

            foreach (KeyValuePair<SerializableVector3,SavedPlaceable> keyValuePair in saveData.builtPlaceables)
            {
                
                IPlaceable placeable = Instantiate(placeableObjectsPrefabs[keyValuePair.Value.id], keyValuePair.Key.ToVector(), Quaternion.Euler(keyValuePair.Value.eulerRotation.ToVector()), buildObjectsParent).GetComponent<IPlaceable>();
                placeable.FullyPlaced(this);
                placeable.SetOrigin(keyValuePair.Value.origin.ToVector());
                placeable.SetTakenGridPositions(keyValuePair.Value.GetTakenGridPositions());
                placeable.LoadSavedInformation(keyValuePair.Value.variableInformation);

                builtObjects.Add(keyValuePair.Key, new SavedPlaceable(keyValuePair.Value.id,keyValuePair.Value.eulerRotation.ToVector(),keyValuePair.Value.origin.ToVector(),keyValuePair.Value.GetTakenGridPositions(), placeable));
            }

        }

        [System.Serializable]
        private class SaveData
        {
            public bool[,] obstructedList;
            public Dictionary<SerializableVector3, SavedPlaceable> builtPlaceables;
            public SaveData(bool[,] obstructedList, Dictionary<SerializableVector3, SavedPlaceable> builtPlaceables)
            {
                this.obstructedList = obstructedList;
                this.builtPlaceables = builtPlaceables;
            }
        }

        [System.Serializable]
        private class SavedPlaceable
        {
            public int id;
            public SerializableVector3 eulerRotation;
            public SerializableVector2Int origin;
            public SerializableVector2Int[] takenGridPositions;
            public object variableInformation;

            public SavedPlaceable(int id, Vector3 eulerRotation, Vector2Int origin, Vector2Int[] takenGridPositions, object variableInformation)
            {
                this.id = id;
                this.eulerRotation = new SerializableVector3(eulerRotation);
                this.origin = new SerializableVector2Int(origin);
                this.variableInformation = variableInformation;

                this.takenGridPositions = new SerializableVector2Int[takenGridPositions.Length];
                for (int index = 0; index < takenGridPositions.Length; index++)
                {
                    this.takenGridPositions[index] = new SerializableVector2Int(takenGridPositions[index]);
                }
            }
            
            public Vector2Int[] GetTakenGridPositions()
            {
                Vector2Int[] takenGridPositionsNew = new Vector2Int[takenGridPositions.Length];
                for (int index = 0; index < takenGridPositions.Length; index++)
                {
                    takenGridPositionsNew[index] = takenGridPositions[index].ToVector();
                }
                return takenGridPositionsNew;
            }

            public SavedPlaceable Copy()
            {
                return new SavedPlaceable(id, eulerRotation.ToVector(), origin.ToVector(), GetTakenGridPositions(),variableInformation);
            }
        }
    }

}