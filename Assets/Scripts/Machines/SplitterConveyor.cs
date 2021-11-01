using CON.Core;
using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace CON.Machines
{
    public class SplitterConveyor : MonoBehaviour, IPlaceable
    {
        public bool isRightToLeft = true;
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [SerializeField] Element elementPlacementRequirement;
        [SerializeField] Transform[] pathOfElement;
        [SerializeField] float forceToApplyForward;
        [SerializeField] float forceToApplySide;
        [SerializeField] GameObject[] directionArrows;
        [SerializeField] Transform hook;
        [SerializeField] Transform hookPosition;
        [SerializeField] Animation hookAnimation; // TODO: Speed up animation
        [SerializeField] AudioClip[] conveyorSounds;

        [HideInInspector] public event Action OnSplitterClicked;
        [HideInInspector] public event Action OnFullyPlaced;

        int elementRatio = 2;
        int elementCounter = 1;
        Vector2Int gridOrigin;
        string hash;
        Builder player;
        AudioSourceManager audioLoop;
        bool isFullyPlaced = false;

        private void Awake()
        {
            audioLoop = GetComponent<AudioSourceManager>();
        }

        private void Start()
        {
            UpdateHookPosition();
            if (player != null) OnBuildModeChange(false);
        }
        private void OnDisable()
        {
            if (player == null) return;
            player.onBuildModeChange -= OnBuildModeChange;
        }
        private void ToggleHookPosition()
        {
            isRightToLeft = !isRightToLeft;
            UpdateHookPosition();
        }
        public void UpdateHookPosition()
        {
            if (!isRightToLeft)
            {
                hook.localPosition = new Vector3(hook.localPosition.x, hook.localPosition.y, 6.5f);
            }
            else
            {
                hook.localPosition = new Vector3(hook.localPosition.x, hook.localPosition.y, 8);
            }
        }
        public void UpdateElementRatio(int afterHowManyElements)
        {
            elementRatio = afterHowManyElements + 1;
            elementCounter = 1;
        }
        private void OnBuildModeChange(bool isActive)
        {
            foreach(GameObject arrow in directionArrows)
            {
                arrow.SetActive(isActive);
            }
        }

        public void OnElementRightEnter(ElementPickup elementPickup)
        {
            if (!isRightToLeft || !isFullyPlaced || hookAnimation.isPlaying) return;
            Rigidbody rigidbody = elementPickup.GetComponent<Rigidbody>();
            StartCoroutine(MoveElement(rigidbody));
            
        }
        public void OnElementLeftEnter(ElementPickup elementPickup)
        {
            if (isRightToLeft || !isFullyPlaced || hookAnimation.isPlaying) return;
            Rigidbody rigidbody = elementPickup.GetComponent<Rigidbody>();
            StartCoroutine(MoveElement(rigidbody));
        }
        private IEnumerator MoveElement(Rigidbody rigidbody)
        {
            if (elementCounter == elementRatio)
            {
                elementCounter = 1;
                rigidbody.isKinematic = true;
                rigidbody.transform.parent = hook;
                rigidbody.transform.position = hookPosition.position;

                if(isRightToLeft) hookAnimation.Play("Seperator_Conveyor_Right");
                else hookAnimation.Play("Seperator_Conveyor_Left");

                yield return new WaitForSeconds(.5f);

                rigidbody.isKinematic = false;
                rigidbody.transform.parent = null;

                
            }
            else if (!rigidbody.isKinematic) elementCounter++;
        }
        
        private void OnCollisionStay(Collision collision)
        {
            if (!IsElementPickup(collision)) return;

            Rigidbody rigidbody = collision.transform.GetComponentInParent<Rigidbody>();
            

            if (!rigidbody.isKinematic)
            {
                rigidbody.velocity = (pathOfElement[1].position - pathOfElement[0].position).normalized * forceToApplyForward;
            }
        }

        private bool IsElementPickup(Collision collision)
        {
            ElementPickup elementPickup = collision.transform.GetComponentInParent<ElementPickup>();
            if (elementPickup == null) return false;
            return true;
        }

        // Interface Implementations
        private void OnMouseDown()
        {
            if (!isFullyPlaced || EventSystem.current.IsPointerOverGameObject() || player.IsDemolishMode()) return;

            if(OnSplitterClicked != null) OnSplitterClicked();
        }

        public void FullyPlaced(Builder player)
        {
            GetComponent<NavMeshObstacle>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
            this.player = player;
            isFullyPlaced = true;
            audioLoop.StartLooping(conveyorSounds);
            OnFullyPlaced();
            player.onBuildModeChange += OnBuildModeChange;
        }

        public InventoryItem[] GetNeededBuildingElements()
        {
            return elementBuildingRequirements;
        }
        public Element GetElementPlacementRequirement()
        {
            return elementPlacementRequirement;
        }
        public Vector2Int GetOrigin()
        {
            return gridOrigin;
        }

        public Vector2Int[] GetTakenGridPositions()
        {
            return takenGridPositions;
        }
        public void SetTakenGridPositions(Vector2Int[] takenGridPositions)
        {
            this.takenGridPositions = takenGridPositions;
        }

        public void SetOrigin(Vector2Int gridOrigin)
        {
            this.gridOrigin = gridOrigin;
        }
        public void ChangeVersion()
        {
            ToggleHookPosition();
        }
        public void SaveHash(string hash)
        {
            this.hash = hash;
        }
        public string GetHash()
        {
            return hash;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public object GetInformationToSave()
        {
            return new SavedSeperatorConveyor(isRightToLeft,elementCounter,elementRatio);
        }

        public void LoadSavedInformation(object savedInformation)
        {
            if (savedInformation == null) return;
            try // TODO: Remove Try Catch block after V0.7
            {
                SavedSeperatorConveyor savedSeperatorConveyor = (SavedSeperatorConveyor)savedInformation;

                isRightToLeft = savedSeperatorConveyor.isRightToLeft;
                elementCounter = savedSeperatorConveyor.elementCounter;
                elementRatio = savedSeperatorConveyor.elementRatio;

                UpdateHookPosition();
            }
            catch (InvalidCastException)
            {
                Debug.LogWarning("The loaded save is old (Splitter Hook placements might be invalid) and will cause errors after alpha version 0.7. Save manually again and delete this save!");
            }
            
        }

        [System.Serializable]
        private class SavedSeperatorConveyor
        {
            public bool isRightToLeft;
            public int elementRatio;
            public int elementCounter;

            public SavedSeperatorConveyor(bool isRightToLeft, int elementCounter,int elementRatio)
            {
                this.isRightToLeft = isRightToLeft;
                this.elementCounter = elementCounter;
                this.elementRatio = elementRatio;
            }
        }
    }

}