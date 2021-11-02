using Astutos.Saving;
using CON.BuildingGrid;
using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CON.Core;
using UnityEngine.EventSystems;

namespace CON.Machines
{
    public class Builder : MonoBehaviour, ISaveable
    {
        [SerializeField] GameObject[] placeableObjectsPrefabs;
        [Header("References")]
        [SerializeField] BuildingGridMesh buildingGridMesh;
        [SerializeField] GameObject coreGameObject;
        [SerializeField] AudioSource audioSource;
        [SerializeField] Transform buildObjectsParent;
        [Header("Smoothing Parameters")]
        [SerializeField] float rotationTime = .2f;
        [SerializeField] float maxSmoothMove = 2f;
        [SerializeField] float distanceDivider = 10f;
        [Header("Sound Effects")]
        [SerializeField] AudioClip[] placementSounds;
        [SerializeField] AudioClip[] demolishSounds;
        [SerializeField] AudioClip[] rotationSounds;
        [Header("Key Mappings")]
        [SerializeField] string deactivatePlacementButtonName;
        [SerializeField] string changeVersionButtonName;
        [SerializeField] string toggleDemolishModeButtonName;
        [SerializeField] string rotateLeftButtonName;
        [SerializeField] string rotateRightButtonName;


        public event Action<bool> onDemolishModeChange;
        public event Action<bool> onBuildModeChange;

        BuildingGridManager buildingGrid;

        bool isDemolishMode = false;
        bool isBuildMode = false;
        bool isPlacementMode = false;

        bool isRotating;
        Quaternion currentRotationGoal;

        GameObject currentMachinePrefab;
        GameObject currentMachine;
        IPlaceable currentPlaceable;
        PlaceableInformation currentPlaceableInformation;
        Vector3 currentMoveGoal;
        Dictionary<string,SavedPlaceable> builtObjects = new Dictionary<string, SavedPlaceable>();

        Inventory inventory;
        CloseButtonManager closeButtonManager;
        SettingsManager settingsManager;
        InputAllowance inputAllowance;

        KeyCode deactivatePlacementButton;
        KeyCode changeVersionButton;
        KeyCode toggleDemolishModeButton;
        KeyCode rotateLeftButton;
        KeyCode rotateRightButton;

        private void Awake()
        {
            inventory = GetComponent<Inventory>();
            settingsManager = FindObjectOfType<SettingsManager>();
            closeButtonManager = coreGameObject.GetComponent<CloseButtonManager>();
            inputAllowance = coreGameObject.GetComponent<InputAllowance>();

            settingsManager.OnInputButtonsChanged += UpdateButtonMapping;
        }

        private void Start()
        {
            if (buildingGrid == null) BuildDefaultBuildingGrid();
            if (onBuildModeChange != null) onBuildModeChange(false);
        }


        private void Update()
        {
            HandleInput();
            HandleModes();
        }


        private void HandleInput()
        {
            if (!isBuildMode) return;

            if (isPlacementMode && Input.GetKeyDown(deactivatePlacementButton))
            {
                DeactivatePlacementModeDestruction();
            }
            if(isPlacementMode && Input.GetKeyDown(changeVersionButton))
            {
                currentPlaceable.ChangeVersion();
            }
            if (Input.GetKeyDown(toggleDemolishModeButton))
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

            closeButtonManager.AddFunction(DeactivatePlacementModeDestruction, "placement");

            isPlacementMode = true;
            currentMachinePrefab = machine;
            currentMachine = Instantiate(machine);
            currentPlaceable = currentMachine.GetComponent<IPlaceable>();
            currentPlaceableInformation = currentPlaceable.GetPlaceableInformation();
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

        public void ToggleBuildMode()
        {
            isBuildMode = !isBuildMode;
            SetActiveBuildMode();
        }
        private void SetActiveBuildMode()
        {
            if(onBuildModeChange != null) onBuildModeChange(isBuildMode);
            inputAllowance.SetActiveZoom(isBuildMode);
            buildingGridMesh.SetActiveMesh(isBuildMode);

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
                closeButtonManager.AddFunction(() => SetActiveDemolishMode(false), this.GetHashCode().ToString());
                DeactivatePlacementModeDestruction();
            }
            else closeButtonManager.RemoveFunction(this.GetHashCode().ToString());

            isDemolishMode = isActive;
            onDemolishModeChange(isActive);
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
                buildingGrid.GetGridPosition(raycastHit.point, out x, out y);

                if (Input.GetMouseButtonDown(0) && buildingGrid.IsObstructed(x, y) && !EventSystem.current.IsPointerOverGameObject())
                {
                   
                    IPlaceable placedMachine = raycastHit.collider.GetComponentInParent<IPlaceable>();

                    if (placedMachine == null) return;

                    PlaceableInformation placeableInformation = placedMachine.GetPlaceableInformation();

                    foreach (Vector2Int takenGridPosition in placeableInformation.takenGridPositions)
                    {
                        buildingGrid.SetObstructed(placeableInformation.buildingGridOrigin.x + takenGridPosition.x, placeableInformation.buildingGridOrigin.y + takenGridPosition.y, false);
                    }
                    foreach (InventoryItem inventoryItem in placeableInformation.buildingRequirements)
                    {
                        inventory.EquipItem(inventoryItem);
                    }
                    buildingGridMesh.UpdateTexture(buildingGrid.GetBuildingGridTexture());

                    builtObjects.Remove(placeableInformation.uniqueIdentifier);
                    Destroy(placedMachine.GetGameObject());
                    audioSource.PlayOneShot(demolishSounds[GetRandomArrayIndex(demolishSounds)]);
                }
            }
        }
        private void HandleRotation()
        {
            if ((Input.GetKeyDown(rotateLeftButton) || Input.mouseScrollDelta.y >= 1f) && !isRotating)
            {
                audioSource.PlayOneShot(rotationSounds[GetRandomArrayIndex(rotationSounds)]);
                RotateLeft();
            }
            if ((Input.GetKeyDown(rotateRightButton) || Input.mouseScrollDelta.y <= -1f) && !isRotating)
            {
                audioSource.PlayOneShot(rotationSounds[GetRandomArrayIndex(rotationSounds)]);
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
                buildingGrid.GetGridPosition(raycastHit.point, out x, out y);

                
                if (IsObstructedAll(x, y)) return;

                currentMoveGoal = buildingGrid.GetWorldPositionCenter(x, y);
                
                
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
            foreach (Vector2Int takenGridPosition in currentPlaceableInformation.takenGridPositions)
            {
                isObstructed = buildingGrid.IsObstructed(x + takenGridPosition.x, y + takenGridPosition.y);
                if (isObstructed) return isObstructed;
            }
            return isObstructed;
        }
        private void Placement(int x, int y)
        {
            
            if (!IsPlacementPossible(x, y)) return;
            
            RemoveElements(currentPlaceable);

            foreach (Vector2Int takenGridPosition in currentPlaceableInformation.takenGridPositions)
            {
                buildingGrid.SetObstructed(x + takenGridPosition.x, y + takenGridPosition.y, true);
            }

            currentMachine.transform.position = currentMoveGoal;
            currentMachine.transform.rotation = currentRotationGoal;
            isRotating = false;
            currentPlaceableInformation.buildingGridOrigin = new Vector2Int(x, y);
            currentPlaceable.FullyPlaced(this);
            currentPlaceableInformation.uniqueIdentifier = currentPlaceable.GetHashCode().ToString();
            builtObjects.Add(currentPlaceableInformation.uniqueIdentifier, new SavedPlaceable(GetPlaceableObjectsID(currentMachinePrefab), currentMachine.transform.position, currentMachine.transform.eulerAngles, new Vector2Int(x,y),currentPlaceableInformation.takenGridPositions, currentPlaceable));
            audioSource.PlayOneShot(placementSounds[GetRandomArrayIndex(placementSounds)]);

            ReenablePlacementMode();
        }


        
        private bool IsPlacementPossible(int x, int y)
        {
            if (EventSystem.current.IsPointerOverGameObject()) return false;

            if (!AreEnoughElements()) return false;
            
            if (currentPlaceableInformation.placementRequirement != null &&!buildingGrid.HasElement(x, y, currentPlaceableInformation.placementRequirement)) return false;

            

            return true;
        }
        
        private bool AreEnoughElements()
        {
            bool enough = true;
            foreach (InventoryItem inventoryItem in currentPlaceableInformation.buildingRequirements)
            {
                enough = inventory.HasItem(inventoryItem);
                if (!enough) return enough;
            }
            return enough;
        }

        private void RemoveElements(IPlaceable currentPlaceable)
        {
            foreach (InventoryItem inventoryItem in currentPlaceableInformation.buildingRequirements)
            {
                inventory.RemoveItem(inventoryItem);
            }
        }
        private void DeactivatePlacementModeDestruction()
        {
            closeButtonManager.RemoveFunction("placement");
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
            currentPlaceableInformation = null;
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
            for (int index = 0; index < currentPlaceableInformation.takenGridPositions.Length; index++)
            {
                int x = currentPlaceableInformation.takenGridPositions[index].x;
                int y = currentPlaceableInformation.takenGridPositions[index].y * -1;
                currentPlaceableInformation.takenGridPositions[index] = new Vector2Int(y, x);
            }
            currentRotationGoal = Quaternion.Euler(new Vector3(0, currentMachine.transform.localEulerAngles.y - 90, 0));
            StartCoroutine(StartRotation(currentRotationGoal));
        }
        private void RotateRight(bool isSmoothed)
        {
            for (int index = 0; index < currentPlaceableInformation.takenGridPositions.Length; index++)
            {
                int x = currentPlaceableInformation.takenGridPositions[index].x * -1;
                int y = currentPlaceableInformation.takenGridPositions[index].y ;
                currentPlaceableInformation.takenGridPositions[index] = new Vector2Int(y, x);
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

        private void BuildDefaultBuildingGrid()
        {
            Texture2D texture = GetDefault2DTexture();
            buildingGrid = new BuildingGridManager(BuildingGridAssetManager.GetGrid(), texture);
            buildingGridMesh.InitiatePlane(texture);
        }

        private void UpdateButtonMapping()
        {
            deactivatePlacementButton = settingsManager.GetKey(deactivatePlacementButtonName);
            changeVersionButton = settingsManager.GetKey(changeVersionButtonName);
            toggleDemolishModeButton = settingsManager.GetKey(toggleDemolishModeButtonName);
            rotateLeftButton = settingsManager.GetKey(rotateLeftButtonName);
            rotateRightButton = settingsManager.GetKey(rotateRightButtonName);
        }
        private int GetRandomArrayIndex(Array array)
        {
            return UnityEngine.Random.Range(0, array.Length);
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
                    obstructedList[x, y] = buildingGrid.gridArray[x, y].obstructed;
                }
            }

            Dictionary<string, SavedPlaceable> newBuiltObjects = new Dictionary<string, SavedPlaceable>();

            foreach(KeyValuePair<string, SavedPlaceable> keyValuePair in builtObjects)
            {
                newBuiltObjects.Add(keyValuePair.Key, new SavedPlaceable(keyValuePair.Value.id, keyValuePair.Value.worldPosition.ToVector(), keyValuePair.Value.eulerRotation.ToVector(), keyValuePair.Value.origin.ToVector(), keyValuePair.Value.GetTakenGridPositions(), ((IPlaceable)keyValuePair.Value.variableInformation).GetInformationToSave()));
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

            buildingGridMesh.InitiatePlane(gridTexture);
            buildingGrid = new BuildingGridManager(gridArray,gridTexture);

            foreach (KeyValuePair<string, SavedPlaceable> keyValuePair in saveData.builtPlaceables)
            {
                IPlaceable placeable = Instantiate(placeableObjectsPrefabs[keyValuePair.Value.id], keyValuePair.Value.worldPosition.ToVector(), Quaternion.Euler(keyValuePair.Value.eulerRotation.ToVector()), buildObjectsParent).GetComponent<IPlaceable>();
                
                PlaceableInformation placeableInformation = placeable.GetPlaceableInformation();

                placeableInformation.buildingGridOrigin = keyValuePair.Value.origin.ToVector();
                placeableInformation.takenGridPositions = keyValuePair.Value.GetTakenGridPositions();
                placeableInformation.uniqueIdentifier = keyValuePair.Key;

                placeable.LoadSavedInformation(keyValuePair.Value.variableInformation);
                placeable.FullyPlaced(this);

                builtObjects.Add(keyValuePair.Key, new SavedPlaceable(keyValuePair.Value.id,keyValuePair.Value.worldPosition.ToVector(),keyValuePair.Value.eulerRotation.ToVector(),keyValuePair.Value.origin.ToVector(),keyValuePair.Value.GetTakenGridPositions(), placeable));
            }

        }

        [System.Serializable]
        private class SaveData
        {
            public bool[,] obstructedList;
            public Dictionary<string, SavedPlaceable> builtPlaceables;
            public SaveData(bool[,] obstructedList, Dictionary<string, SavedPlaceable> builtPlaceables)
            {
                this.obstructedList = obstructedList;
                this.builtPlaceables = builtPlaceables;
            }
        }

        [System.Serializable]
        private class SavedPlaceable
        {
            public int id;
            public SerializableVector3 worldPosition;
            public SerializableVector3 eulerRotation;
            public SerializableVector2Int origin;
            public SerializableVector2Int[] takenGridPositions;
            public object variableInformation;

            public SavedPlaceable(int id, Vector3 worldPosition, Vector3 eulerRotation, Vector2Int origin, Vector2Int[] takenGridPositions, object variableInformation)
            {
                this.id = id;
                this.worldPosition = new SerializableVector3(worldPosition);
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
                return new SavedPlaceable(id, worldPosition.ToVector(), eulerRotation.ToVector(), origin.ToVector(), GetTakenGridPositions(),variableInformation);
            }
        }
    }

}