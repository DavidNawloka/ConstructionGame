using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CON.Progression
{
    public class FindableNote : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public FindableNoteIdentifier noteType;
        [SerializeField] NoteDistance[] perfectNoteDistances;
        [SerializeField] float maxDistanceDifference = 5f;
        [Header("Window")]
        [SerializeField] float windowWidth;
        [SerializeField] float windowHeight;

        bool followMouse = false;
        bool keyPlacementFound = false;
        Vector3 initialMousePosition;
        RectTransform rectTransform;

        [HideInInspector] public UnityEvent<bool> OnMovementStatusChange;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (!followMouse || keyPlacementFound) return;
            UpdateOwnPosition();
        }
        private void UpdateOwnPosition()
        {
            transform.position = new Vector3(
                    Mathf.Clamp(Input.mousePosition.x + initialMousePosition.x, windowWidth / 2 + 460, Screen.currentResolution.width - windowWidth / 2),
                    Mathf.Clamp(Input.mousePosition.y + initialMousePosition.y, windowHeight / 2, Screen.currentResolution.height - windowHeight / 2),
                    0);
        }

        public bool IsKeyPlacement() // TODO: Optimize
        {
            foreach(NoteDistance noteDistance in perfectNoteDistances)
            {
                foreach (Transform findableNoteTransform in transform.parent)
                {
                    if (findableNoteTransform.GetComponent<FindableNote>().noteType == noteDistance.noteType)
                    {
                        float xDistance = findableNoteTransform.GetComponent<RectTransform>().anchoredPosition.x - rectTransform.anchoredPosition.x;
                        float yDistance = findableNoteTransform.GetComponent<RectTransform>().anchoredPosition.y - rectTransform.anchoredPosition.y;

                        if (xDistance > noteDistance.distance.x - maxDistanceDifference && xDistance < noteDistance.distance.x + maxDistanceDifference)
                        {
                            if (yDistance > noteDistance.distance.y - maxDistanceDifference && yDistance < noteDistance.distance.y + maxDistanceDifference)
                            {
                                break;
                            }
                        }
                        
                        return false;
                    }
                }
            }
            return true;
        }

        public void KeyPlacementFound()
        {
            keyPlacementFound = true;
            GetComponent<Button>().enabled = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        public Vector2 GetPerfectDistanceTo(int noteIdentifierIndex)
        {

            foreach(NoteDistance noteDistance in perfectNoteDistances)
            {
                if (noteDistance.noteType == (FindableNoteIdentifier)noteIdentifierIndex) return noteDistance.distance;
            }
            return Vector2.zero;
        }
        public RectTransform GetRectTransform()
        {
            return rectTransform;
        }

#if UNITY_EDITOR
        public void UpdatePerfectKeyDistances()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty listProperty = serializedObject.FindProperty("perfectNoteDistances");

            for (int x = 0; x < listProperty.arraySize; x++)
            {
                SerializedProperty property = listProperty.GetArrayElementAtIndex(x);


                SerializedProperty serializedDistance = property.FindPropertyRelative("distance");
                SerializedProperty serializedEnum= property.FindPropertyRelative("noteType");

                foreach(Transform findableNoteTransform in transform.parent)
                {
                    if((int)findableNoteTransform.GetComponent<FindableNote>().noteType == serializedEnum.enumValueIndex)
                    {
                        serializedDistance.vector2Value = new Vector2(
                            findableNoteTransform.GetComponent<RectTransform>().anchoredPosition.x - GetComponent<RectTransform>().anchoredPosition.x,
                            findableNoteTransform.GetComponent<RectTransform>().anchoredPosition.y - GetComponent<RectTransform>().anchoredPosition.y);
                    }
                }
                
            }
            serializedObject.ApplyModifiedProperties();

        }
#endif
        // Interface Implementations
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null && !keyPlacementFound)
            {
                OnMovementStatusChange.Invoke(true);
                followMouse = true;
                initialMousePosition = transform.position - Input.mousePosition;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnMovementStatusChange.Invoke(false);
            followMouse = false;
        }

        [System.Serializable]
        public class NoteDistance
        {
            public Vector2 distance;
            public FindableNoteIdentifier noteType;
        }
    }

}