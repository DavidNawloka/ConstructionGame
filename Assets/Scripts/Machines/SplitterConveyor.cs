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
    public class SplitterConveyor : MonoBehaviour, IPlaceable, IRaycastable
    {
        public bool isRightToLeft = true;
        [SerializeField] PlaceableInformation placeableInformation;
        [SerializeField] Transform[] pathOfElement;
        [SerializeField] float forceToApplyForward;
        [SerializeField] GameObject[] directionArrows;
        [SerializeField] Transform[] hooks;
        [SerializeField] Animation hookAnimation; // TODO: Speed up animation
        [SerializeField] AudioClip[] conveyorSounds;

        [HideInInspector] public event Action OnSplitterClicked;
        [HideInInspector] public event Action OnFullyPlaced;

        int elementRatio = 2;
        int elementCounter = 1;
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
                foreach(Transform hook in hooks)
                {
                    hook.parent.localPosition = new Vector3(hook.parent.localPosition.x, hook.parent.localPosition.y, 6.5f);
                }
            }
            else
            {
                foreach(Transform hook in hooks)
                {
                    hook.parent.localPosition = new Vector3(hook.parent.localPosition.x, hook.parent.localPosition.y, 8);
                }
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
                rigidbody.transform.parent = hooks[0];
                rigidbody.transform.localPosition = Vector3.zero;

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
            if (!IsElementPickup(collision) || !isFullyPlaced) return;

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
        public CursorType GetCursorType()
        {
            return CursorType.Placeable;
        }

        public bool InRange(Transform player)
        {
            return isFullyPlaced;
        }

        public void HandleInteractionClick(Transform player)
        {
            if (!isFullyPlaced || EventSystem.current.IsPointerOverGameObject() || this.player.IsDemolishMode()) return;

            if (OnSplitterClicked != null) OnSplitterClicked();
        }
        public void PlacementStatusChange(Builder player, PlacementStatus placementStatus)
        {
            switch (placementStatus)
            {
                case PlacementStatus.startingPlacement:
                    GetComponent<NavMeshObstacle>().enabled = true;
                    player.onBuildModeChange += OnBuildModeChange;
                    this.player = player;
                    break;
                case PlacementStatus.endingPlacement:
                    GetComponent<BoxCollider>().enabled = true;
                    isFullyPlaced = true;
                    audioLoop.StartLooping(conveyorSounds);
                    OnFullyPlaced();
                    break;
                case PlacementStatus.startingDemolishment:
                    isFullyPlaced = false;
                    break;
                case PlacementStatus.endingDemolishment:
                    Destroy(gameObject);
                    break;
            }
        }
        public void ChangeVersion()
        {
            ToggleHookPosition();
        }

        public PlaceableInformation GetPlaceableInformation()
        {
            return placeableInformation;
        }
        public object GetInformationToSave()
        {
            return new SavedSeperatorConveyor(isRightToLeft,elementCounter,elementRatio);
        }

        public void LoadSavedInformation(object savedInformation)
        {
            if (savedInformation == null) return;
            SavedSeperatorConveyor savedSeperatorConveyor = (SavedSeperatorConveyor)savedInformation;

            isRightToLeft = savedSeperatorConveyor.isRightToLeft;
            elementCounter = savedSeperatorConveyor.elementCounter;
            elementRatio = savedSeperatorConveyor.elementRatio;

            UpdateHookPosition();
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