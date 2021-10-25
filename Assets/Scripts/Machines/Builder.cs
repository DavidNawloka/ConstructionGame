using Astutos.Saving;
using CON.BuildingGrid;
using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CON.Core;
using UnityEngine.EventSystems;
using CON.UI; // TODO: Fix questionable dependency

namespace CON.Machines
{
    public class Builder : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform buildObjectsParent;
        [SerializeField] GameObject[] placeableObjectsPrefabs;
        [SerializeField] float rotationTime = .2f;
        [SerializeField] float maxSmoothMove = 2f;
        [SerializeField] float distanceDivider = 10f;
        [Header("Sound Effects")]
        [SerializeField] AudioClip[] placementSounds;
        [SerializeField] AudioClip[] demolishSounds;
        [SerializeField] AudioClip[] rotationSounds;


        public event Action<bool> onDemolishModeChange;
        public event Action<bool> onBuildModeChange;

        BuildingGridManager grid;
        BuildingGridMesh gridMesh;
        

        bool isPaused;
        bool isDemolishMode = false;
        bool isBuildMode = false;
        bool isPlacementMode = false;

        bool isRotating;
        Quaternion currentRotationGoal;

        GameObject currentMachinePrefab;
        GameObject currentMachine;
        IPlaceable currentPlaceable;
        Vector2Int[] takenGridPositions;
        Vector3 currentMoveGoal;
        Dictionary<SerializableVector3,SavedPlaceable> builtObjects = new Dictionary<SerializableVector3, SavedPlaceable>();

        Inventory inventory;
        AudioSourceManager audioSourceManager;
        UserInterfaceManager userInterfaceManager;
        EscManager escManager;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            audioSourceManager = GetComponent<AudioSourceManager>();
            gridMesh = FindObjectOfType<BuildingGridMesh>();
            userInterfaceManager = FindObjectOfType<UserInterfaceManager>();
            escManager = FindObjectOfType<EscManager>();
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
            if (isPaused) return;
            HandleInput();
            HandleModes();
        }


        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                userInterfaceManager.ToggleUI(1);
            }

            if (!isBuildMode) return;

            if (isPlacementMode && Input.GetKeyDown(KeyCode.C))
            {
                DeactivatePlacementModeDestruction();
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
                SmoothMovePlaceable();
            }
        }

        public void ActivatePlacementMode(GameObject machine)
        {
            if (currentMachine != null) Destroy(currentMachine);
            if (isDemolishMode) SetActiveDemolishMode(false);

            escManager.AddEscFunction(DeactivatePlacementModeDestruction, "placement");

            isPlacementMode = true;
            currentMachinePrefab = machine;
            currentMachine = Instantiate(machine);
            currentPlaceable = currentMachine.GetComponent<IPlaceable>();
            takenGridPositions = currentPlaceable.GetTakenGridPositions();
            currentMachine.transform.position = GetNewMachinePosition();
            currentMachine.transform.parent = buildObjectsParent;
        }

        private Vector3 GetNewMachinePosition()
        {
            RaycastHit rayCastHit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out rayCastHit))
            {
                return rayCastHit.point;
            }
            return transform.position;
        }

        public void OnPauseChange(bool isPaused)
        {
            this.isPaused = isPaused;
        }

        public void ToggleBuildMode()
        {
            isBuildMode = !isBuildMode;
            SetActiveBuildMode();
        }
        private void SetActiveBuildMode()
        {
            onBuildModeChange(isBuildMode);
            gridMesh.SetActiveMesh(isBuildMode);

            if (isDemolishMode) SetActiveDemolishMode(isBuildMode);
            if (isPlacementMode) DeactivatePlacementModeDestruction();
        }
        public void ToggleDemolishMode()
        {
            SetActiveDemolishMode(!isDemolishMode);
        }
        public void SetActiveDemolishMode(bool isActive)
        {
            if (isActive)
            {
                escManager.AddEscFunction(() => SetActiveDemolishMode(false), this.GetHashCode().ToString());
                DeactivatePlacementModeDestruction();
            }
            else escManager.RemoveESCFunction(this.GetHashCode().ToString());

            isDemolishMode = isActive;
            onDemolishModeChange(isActive);
        }
        public BuildingGridMesh GetGridMesh()
        {
            return gridMesh;
        }
        
        public bool IsDemolishMode()
        {
            return isDemolishMode;
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

                if (Input.GetMouseButtonDown(0) && grid.IsObstructed(x, y) && !EventSystem.current.IsPointerOverGameObject())
                {
                   
                    IPlaceable placedMachine = raycastHit.collider.GetComponentInParent<IPlaceable>();

                    if (placedMachine == null) return;

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
                            builtObjects.Remove(keyValuePair.Key);
                            break;
                        }
                    }

                    Destroy(placedMachine.GetGameObject());
                    audioSourceManager.PlayOnceFromMultiple(demolishSounds);
                }
            }
        }
        private void HandleRotation()
        {
            if ((Input.GetKeyDown(KeyCode.Q) || Input.mouseScrollDelta.y >= 1f) && !isRotating)
            {
                audioSourceManager.PlayOnceFromMultiple(rotationSounds);
                RotateLeft();
            }
            if ((Input.GetKeyDown(KeyCode.E) || Input.mouseScrollDelta.y <= -1f) && !isRotating)
            {
                audioSourceManager.PlayOnceFromMultiple(rotationSounds);
                RotateRight(true);
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

                currentMoveGoal = grid.GetWorldPositionCenter(x, y);
                
                
                if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
                {
                    
                    Placement(x, y);
                }
            }
        }
        private void SmoothMovePlaceable()
        {
            float distance = Vector3.Distance(currentMachine.transform.position, currentMoveGoal);
            currentMachine.transform.position = Vector3.MoveTowards(currentMachine.transform.position, currentMoveGoal, ((maxSmoothMove*distance)/ distanceDivider) *Time.deltaTime);
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

            currentMachine.transform.position = currentMoveGoal;
            currentMachine.transform.rotation = currentRotationGoal;
            isRotating = false;
            currentPlaceable.SetOrigin(new Vector2Int(x, y));
            currentPlaceable.FullyPlaced(this);
            builtObjects.Add(new SerializableVector3(currentMachine.transform.position),new SavedPlaceable(GetPlaceableObjectsID(currentMachinePrefab), currentMachine.transform.eulerAngles, new Vector2Int(x,y),takenGridPositions, currentPlaceable));
            audioSourceManager.PlayOnceFromMultiple(placementSounds);

            ReenablePlacementMode();
        }


        
        private bool IsPlacementPossible(int x, int y)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return false;

            if (!AreEnoughElements()) return false;
            
            if (currentPlaceable.GetElementPlacementRequirement() != null &&!grid.HasElement(x, y, currentPlaceable.GetElementPlacementRequirement())) return false;

            

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
        private void DeactivatePlacementModeDestruction()
        {
            escManager.RemoveESCFunction("placement");
            Destroy(currentMachine);
            isPlacementMode = false;
            currentMachine = null;
            currentPlaceable = null;
            currentRotationGoal = Quaternion.Euler(Vector3.one);
        }
        private void ReenablePlacementMode()
        {
            
            isPlacementMode = false;
            int toRotate = (int)currentMachine.transform.localEulerAngles.y / 90;
            currentMachine = null;
            currentPlaceable = null;
            ActivatePlacementMode(currentMachinePrefab);
            for (int i = 0; i < toRotate; i++)
            {
                RotateRight(false);
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
            currentRotationGoal = Quaternion.Euler(new Vector3(0, currentMachine.transform.localEulerAngles.y - 90, 0));
            StartCoroutine(StartRotation(currentRotationGoal));
        }
        private void RotateRight(bool isSmoothed)
        {
            for (int index = 0; index < takenGridPositions.Length; index++)
            {
                int x = takenGridPositions[index].x * -1;
                int y = takenGridPositions[index].y ;
                takenGridPositions[index] = new Vector2Int(y, x);
            }
            currentRotationGoal = Quaternion.Euler(new Vector3(0, currentMachine.transform.localEulerAngles.y + 90, 0));
            if (isSmoothed) StartCoroutine(StartRotation(currentRotationGoal));
            else currentMachine.transform.localRotation = currentRotationGoal;
        }
        IEnumerator StartRotation(Quaternion goal)
        {
            isRotating = true;
            Quaternion initialRotation = currentMachine.transform.localRotation;
            float amount = 0;
            while(amount < rotationTime)
            {
                if (!isRotating) break;
                currentMachine.transform.localRotation = Quaternion.Slerp(initialRotation, goal,amount/rotationTime);
                amount += Time.deltaTime;
                yield return null;
            }
            if(isRotating) currentMachine.transform.localRotation = goal;
            isRotating = false;
        }
        // Interface Implementations


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
            foreach(Transform placeable in buildObjectsParent)
            {
                Destroy(placeable.gameObject);
            }


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