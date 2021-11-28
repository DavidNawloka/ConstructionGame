using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using CON.UI;
using CON.Elements;
using CON.Machines;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] TutorialSection[] tutorialSections;
    [Header("Visualisation")]
    [SerializeField] RectTransform sectionParent;
    [SerializeField] TextMeshProUGUI sectionHead;
    [SerializeField] ObjectiveVisualisation[] objectiveVisualisations;
    [SerializeField] Color objectiveCompleted;
    [SerializeField] Color objectiveUncompleted;
    [SerializeField] float timeToChangeSection = 1f;
    [SerializeField] float defaultObjectiveHeight;
    [Header("Tutorial Section Specific")]
    [SerializeField] Transform player;
    [SerializeField] CinemachineVirtualCamera followCamera;
    [SerializeField] Inventory playerInventory;
    [SerializeField] Machine waterCollector;
    [SerializeField] UserInterfaceManager userInterfaceManager;
    [SerializeField] Transform builtMachinesParent;

    int currentTutorialSectionIndex = 0;
    bool isChangingSection = false;

    Vector3 startingPlayerPosition;
    CinemachineComponentBase componentBase;
    float initialCameraZoom;
    bool hasEquippedItem = false;
    bool hasWaterCollectorElements = false;
    private void Awake()
    {
        startingPlayerPosition = player.position;
        componentBase = followCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        initialCameraZoom = (componentBase as CinemachineFramingTransposer).m_CameraDistance;
        playerInventory.OnInventoryChange.AddListener(TrackItemEquip);

        UpdateSectionVisualisation();
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
    }
    
    private void TrackItemEquip(Inventory inventory)
    {
        hasEquippedItem = inventory.HasAnyItems();
        hasWaterCollectorElements = inventory.HasItem(waterCollector.GetPlaceableInformation().buildingRequirements);
    }
    private bool WasWaterCollectorBuilt()
    {
        foreach(Transform child in builtMachinesParent)
        {
            Machine machine = child.GetComponent<Machine>();
            if (machine == null) continue;
            if (machine.GetPlaceableInformation().placementRequirement == waterCollector.GetPlaceableInformation().placementRequirement && machine.GetFullyPlacedStatus()) return true;
        }
        return false;
    }

    // GENERIC LOGIC USED BY ALL TUTORIAL SECTIONS
    private bool ObjectiveCompleted(int sectionIndex, int objectiveIndex)
    {
        tutorialSections[sectionIndex].objectives[objectiveIndex].completed = true;
        UpdateCurrentObjectiveColoring(objectiveIndex);
        
        if (AllObjectiveComplete(currentTutorialSectionIndex))
        {
            if (tutorialSections[sectionIndex].completed) return true;
            tutorialSections[sectionIndex].completed = true;
            StartCoroutine(ChangeSection());
            return true;
        }
        return false;
    }
    private IEnumerator ChangeSection()
    {
        isChangingSection = true;
        sectionHead.color = objectiveCompleted;
        yield return StartCoroutine(MoveSectionParent());

        currentTutorialSectionIndex = Mathf.Clamp(currentTutorialSectionIndex + 1, 0, tutorialSections.Length + 1);
        UpdateSectionVisualisation();

        yield return StartCoroutine(MoveSectionParent());
        isChangingSection = false; 
    }
    private IEnumerator MoveSectionParent()
    {
        float timer = 0;
        Vector2 initialPosition = sectionParent.anchoredPosition;
        Vector2 targetPosition = new Vector3(
            sectionParent.anchoredPosition.x * -1,
            sectionParent.anchoredPosition.y * -1);
        Vector2 direction = targetPosition - initialPosition;
        while (timer < timeToChangeSection)
        {
            sectionParent.anchoredPosition = initialPosition + direction * timer / timeToChangeSection;
            timer += Time.deltaTime;
            yield return null;
        }
        sectionParent.anchoredPosition = targetPosition;
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
        foreach(GameObject gameObjectToActivate in tutorialSection.gameObjectsToActivate)
        {
            gameObjectToActivate.SetActive(true); // TODO: Slide In with WHOOSH Sound
        }
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
