using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using CON.UI;
using CON.Elements;
using CON.Machines;
using CON.Progression;
using Astutos.Saving;

public class TutorialManager : MonoBehaviour, ISaveable
{
    [SerializeField] TutorialSection[] tutorialSections;
    [Header("Visualisation")]
    [SerializeField] RectTransform sectionParent;
    [SerializeField] RectTransform minimapParent;
    [SerializeField] TextMeshProUGUI sectionHead;
    [SerializeField] ObjectiveVisualisation[] objectiveVisualisations;
    [SerializeField] Color objectiveCompleted;
    [SerializeField] Color objectiveUncompleted;
    [SerializeField] float timeToChangeSection = 1f;
    [SerializeField] float defaultObjectiveHeight;
    [SerializeField] float slideOutDelay = 1f;
    [Header("Tutorial Section Specific")]
    [SerializeField] Transform player;
    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] Inventory playerInventory;
    [SerializeField] Machine waterCollector;
    [SerializeField] UserInterfaceManager userInterfaceManager;
    [SerializeField] Transform builtMachinesParent;
    [SerializeField] Machine woodGatherer;
    [SerializeField] Inventory progressionManagerInventory;
    [SerializeField] Inventory hangingBridgeInventory;
    [SerializeField] WorldBuilding hangingBridge;
    [SerializeField] PlayerCamp playerCamp;
    [SerializeField] Transform noteParent;
    [SerializeField] ProgressionManager progressionManager;
    [SerializeField] Unlockable rockSplitter;

    int currentTutorialSectionIndex = 0;
    bool isChangingSection = false;

    Vector3 startingPlayerPosition;
    CinemachineComponentBase componentBase;
    float initialCameraZoom;
    bool hasEquippedItem = false;
    bool hasWaterCollectorElements = false;
    bool hasWaterInstructionElements = false;
    bool hasWaterCollectorClicked = false;
    bool hasWaterPoduced = false;
    bool hasWoodPoduced = false;
    bool hasHangingBridgeElements = false;
    bool hasPlayerCrossedBridge = false;
    bool machineUnlocked = false;
    bool finishTutorial = false;

    Machine placedWaterCollector;
    private void Awake()
    {
        startingPlayerPosition = player.position;
        componentBase = followCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        initialCameraZoom = (componentBase as CinemachineFramingTransposer).m_CameraDistance;
        playerInventory.OnInventoryChange.AddListener(TrackPlayerItemEquip);
        progressionManagerInventory.OnInventoryChange.AddListener(TrackProgressionItemEquip);
        hangingBridgeInventory.OnInventoryChange.AddListener(TrackHangingBridgeItemEquip);
        progressionManager.OnPlaceableUnlocked.AddListener(MachineUnlocked);
    }
    private void Start()
    {
        if(currentTutorialSectionIndex == 0) UpdateSectionVisualisation();
    }
    private void Update()
    {
        if (isChangingSection) return;

        if (!tutorialSections[0].completed)
        {
            if (Vector3.Distance(startingPlayerPosition,player.position)>1f) ObjectiveCompleted(0, 0);
            if (Mathf.Abs(initialCameraZoom - (componentBase as CinemachineFramingTransposer).m_CameraDistance) > .5f) ObjectiveCompleted(0, 1);
        }
        if (!tutorialSections[1].completed)
        {
            if (userInterfaceManager.IsUserInterfaceTypeActive(1)) ObjectiveCompleted(1, 0);
            if (hasEquippedItem) ObjectiveCompleted(1, 1);
        }
        if (!tutorialSections[2].completed)
        {
            if (hasWaterCollectorElements) ObjectiveCompleted(2, 0);
            if (userInterfaceManager.IsUserInterfaceTypeActive(2)) ObjectiveCompleted(2, 1);
            if (WasWaterCollectorBuilt()) ObjectiveCompleted(2, 2);
        }
        if(placedWaterCollector == null) WasWaterCollectorBuilt();
        if (!tutorialSections[3].completed)
        {
            if (hasWaterInstructionElements) ObjectiveCompleted(3, 0);
            if (hasWaterCollectorClicked) ObjectiveCompleted(3, 1);
            if (hasWaterPoduced) ObjectiveCompleted(3, 2);
        }
        if (!tutorialSections[4].completed)
        {
            if (WasWoodGathererBuilt()) ObjectiveCompleted(4, 0);
            if (hasWoodPoduced) ObjectiveCompleted(4, 1);
        }
        if (!tutorialSections[5].completed)
        {
            if (hasHangingBridgeElements) ObjectiveCompleted(5, 0);
            if (hasPlayerCrossedBridge) ObjectiveCompleted(5, 1);
        }
        if (!tutorialSections[6].completed)
        {
            if (WasCampBuild()) ObjectiveCompleted(6, 0);
            if (noteParent.childCount > 0) ObjectiveCompleted(6, 1);
            if (userInterfaceManager.IsUserInterfaceTypeActive(3)) ObjectiveCompleted(6, 2);
            if (machineUnlocked) ObjectiveCompleted(6, 3);
        }
        if (!tutorialSections[7].completed)
        {
            if (finishTutorial) ObjectiveCompleted(7, 0);
        }

    }
    
    private void TrackPlayerItemEquip(Inventory inventory)
    {
        hasEquippedItem = inventory.HasAnyItems();
        hasWaterCollectorElements = inventory.HasItem(waterCollector.GetPlaceableInformation().buildingRequirements);
        hasWaterInstructionElements = inventory.HasItem(waterCollector.GetPossibleInstructions()[0].requirements[0]);
    }
    private void TrackProgressionItemEquip(Inventory inventory)
    {
        hasWaterPoduced = inventory.HasItem(waterCollector.GetPossibleInstructions()[0].outcome);
        hasWoodPoduced = inventory.HasItem(woodGatherer.GetPossibleInstructions()[0].outcome);
    }
    private void TrackHangingBridgeItemEquip(Inventory inventory)
    {
        hasHangingBridgeElements = inventory.HasItem(hangingBridge.GetUnlockRequirements()[0].inventoryItem);
    }
    private bool WasWaterCollectorBuilt()
    {
        foreach(Transform child in builtMachinesParent)
        {
            Machine machine = child.GetComponent<Machine>();
            if (machine == null) continue;
            if (machine.GetPlaceableInformation().placementRequirement == waterCollector.GetPlaceableInformation().placementRequirement && machine.GetFullyPlacedStatus())
            {
                placedWaterCollector = machine;
                placedWaterCollector.OnMachineClicked += WaterCollectorClicked;
                return true;
            }
        }
        return false;
    }
    private bool WasWoodGathererBuilt()
    {
        foreach (Transform child in builtMachinesParent)
        {
            Machine machine = child.GetComponent<Machine>();
            if (machine == null) continue;
            if (machine.GetPlaceableInformation().placementRequirement == woodGatherer.GetPlaceableInformation().placementRequirement && machine.GetFullyPlacedStatus()) return true;
        }
        return false;
    }
    private bool WasCampBuild()
    {
        foreach (Transform child in builtMachinesParent)
        {
            PlayerCamp playerCamp = child.GetComponent<PlayerCamp>();
            if (playerCamp != null && playerCamp.IsFullyPlaced()) return true;
        }
        return false;
    }

    public void ButtonPress() // Button Event
    {
        finishTutorial = true;
    }

    private void WaterCollectorClicked()
    {
        hasWaterCollectorClicked = true;
    }

    public void PlayerCrossedBridge()
    {
        hasPlayerCrossedBridge = true;
    }
    public void MachineUnlocked(Unlockable unlockable)
    {
        if(unlockable == rockSplitter) machineUnlocked = true;
    }

    // GENERIC LOGIC USED BY ALL TUTORIAL SECTIONS
    private bool ObjectiveCompleted(int sectionIndex, int objectiveIndex)
    {
        tutorialSections[sectionIndex].objectives[objectiveIndex].completed = true;
        UpdateCurrentObjectiveColoring(objectiveIndex);
        
        if (AllObjectiveComplete(sectionIndex))
        {
            if (tutorialSections[sectionIndex].completed) return true;
            tutorialSections[sectionIndex].completed = true;
            if(sectionIndex == currentTutorialSectionIndex) StartCoroutine(ChangeSection());
            return true;
        }
        return false;
    }
    private IEnumerator ChangeSection()
    {
        isChangingSection = true;
        sectionHead.color = objectiveCompleted;

        yield return new WaitForSeconds(slideOutDelay);
        yield return StartCoroutine(MoveSectionParent(sectionParent));

        currentTutorialSectionIndex = NextTutorialSectionIndex();
        if(currentTutorialSectionIndex != -1)
        {
            UpdateSectionVisualisation();

            yield return StartCoroutine(MoveSectionParent(sectionParent));
            isChangingSection = false;
        }
        else
        {
            minimapParent.anchoredPosition = sectionParent.anchoredPosition;
            minimapParent.gameObject.SetActive(true);
            yield return StartCoroutine(MoveSectionParent(minimapParent));
        }
        
    }
    private IEnumerator MoveSectionParent(RectTransform rectTransform)
    {
        float timer = 0;
        Vector2 initialPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector3(
            rectTransform.anchoredPosition.x * -1,
            rectTransform.anchoredPosition.y * -1);
        Vector2 direction = targetPosition - initialPosition;
        while (timer < timeToChangeSection)
        {
            rectTransform.anchoredPosition = initialPosition + direction * timer / timeToChangeSection;
            timer += Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = targetPosition;
    }
    private int NextTutorialSectionIndex()
    {
        for (int sectionIndex = currentTutorialSectionIndex + 1; sectionIndex < tutorialSections.Length; sectionIndex++)
        {
            if (tutorialSections[sectionIndex].completed) ActivateSectionGameObjects(tutorialSections[sectionIndex]);
            else return sectionIndex;
        }
        return -1;
    }
    private void UpdateCurrentObjectiveColoring(int objectiveIndex)
    {
        ObjectiveVisualisation objectiveVisualisation = objectiveVisualisations[objectiveIndex];
        Color colorToShow = objectiveCompleted;
        if (!tutorialSections[currentTutorialSectionIndex].objectives[objectiveIndex].completed) colorToShow = objectiveUncompleted;

        objectiveVisualisation.image.color = colorToShow;
        objectiveVisualisation.tMPro.color = colorToShow;
    }
    private bool AllObjectiveComplete(int sectionIndex)
    {
        foreach(Objective objective in tutorialSections[sectionIndex].objectives)
        {
            if (!objective.completed) return false;
        }
        return true;
    }
    private void UpdateSectionVisualisation()
    {
        TutorialSection tutorialSection = tutorialSections[currentTutorialSectionIndex];
        foreach (ObjectiveVisualisation objectiveVisualisation in objectiveVisualisations)
        {
            objectiveVisualisation.parent.gameObject.SetActive(false);
        }
        sectionHead.text = tutorialSection.name;
        sectionHead.color = objectiveUncompleted;
        for (int objectiveIndex = 0; objectiveIndex < tutorialSection.objectives.Length; objectiveIndex++)
        {
            ObjectiveVisualisation currentVisualisation = objectiveVisualisations[objectiveIndex];
            currentVisualisation.parent.gameObject.SetActive(true);
            currentVisualisation.parent.sizeDelta = new Vector2(currentVisualisation.parent.sizeDelta.x, defaultObjectiveHeight * tutorialSection.objectives[objectiveIndex].objectiveHeightFactor);
            currentVisualisation.tMPro.text = tutorialSection.objectives[objectiveIndex].name;

            Color colorToShow = objectiveUncompleted;
            if (tutorialSection.objectives[objectiveIndex].completed) colorToShow = objectiveCompleted;

            currentVisualisation.tMPro.color = colorToShow;
            currentVisualisation.image.color = colorToShow;
        }
        ActivateSectionGameObjects(tutorialSection);
    }

    private void ActivateSectionGameObjects(TutorialSection tutorialSection)
    {
        foreach (GameObject gameObjectToActivate in tutorialSection.gameObjectsToActivate)
        {
            gameObjectToActivate.SetActive(true); // TODO: Slide In with WHOOSH Sound
        }
    }

    public object CaptureState()
    {
        int sectionsCompleted = 0;
        for (int sectionIndex = 0; sectionIndex < tutorialSections.Length; sectionIndex++)
        {
            if (tutorialSections[sectionIndex].completed) sectionsCompleted++;
        }
        return sectionsCompleted;
    }

    public void RestoreState(object state)
    {
        if (state == null) return;
        currentTutorialSectionIndex = ((int)state);
        for (int sectionIndex = 0; sectionIndex < currentTutorialSectionIndex; sectionIndex++)
        {
            tutorialSections[sectionIndex].completed = true;
        }
        if(currentTutorialSectionIndex == tutorialSections.Length)
        {
            minimapParent.gameObject.SetActive(true);
            sectionParent.gameObject.SetActive(false);
        }
        else UpdateSectionVisualisation();
    }

    [System.Serializable]
    public class TutorialSection
    {
        public string name;
        public bool completed;
        public Objective[] objectives;
        public GameObject[] gameObjectsToActivate;
    }
    [System.Serializable]
    public class Objective
    {
        public string name;
        public float objectiveHeightFactor = 1;
        public bool completed;
    }

    [System.Serializable]
    public class ObjectiveVisualisation
    {
        public RectTransform parent;
        public Image image;
        public TextMeshProUGUI tMPro;
    }
}
