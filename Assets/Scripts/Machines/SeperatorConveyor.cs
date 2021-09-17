using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CON.Machines
{
    public class SeperatorConveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] Vector2Int[] takenGridPositions;
        [SerializeField] InventoryItem[] elementBuildingRequirements;
        [SerializeField] Transform[] pathOfElement;
        [SerializeField] float forceToApplyForward;
        [SerializeField] float forceToApplySide;
        [SerializeField] GameObject[] directionArrows;
        [SerializeField] bool isRightToLeft = true;
        [SerializeField] Transform hook;
        [SerializeField] Animation hookAnimation;


        int elementCounter = 1;
        Vector2Int gridOrigin;
        Builder player;
        private void Start()
        {
            UpdateHookPosition();
        }
        private void ToggleHookPosition()
        {
            isRightToLeft = !isRightToLeft;
            UpdateHookPosition();
        }
        private void UpdateHookPosition()
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
        private void OnDisable()
        {
            if (player == null) return;
            player.onBuildModeChange -= OnBuildModeChange;
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
            if (!isRightToLeft) return;
            Rigidbody rigidbody = elementPickup.GetComponent<Rigidbody>();
            StartCoroutine(MoveElement(rigidbody));
            
        }
        public void OnElementLeftEnter(ElementPickup elementPickup)
        {
            if (isRightToLeft) return;
            Rigidbody rigidbody = elementPickup.GetComponent<Rigidbody>();
            StartCoroutine(MoveElement(rigidbody));
        }
        private IEnumerator MoveElement(Rigidbody rigidbody)
        {
            if (elementCounter == 2)
            {
                rigidbody.isKinematic = true;
                rigidbody.transform.parent = hook;

                if(isRightToLeft) hookAnimation.Play("Seperator_Conveyor_Right");
                else hookAnimation.Play("Seperator_Conveyor_Left");

                yield return new WaitForSeconds(.5f);

                rigidbody.isKinematic = false;
                rigidbody.transform.parent = null;

                elementCounter = 1;
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

        public void FullyPlaced(Builder player)
        {
            GetComponent<NavMeshObstacle>().enabled = true;
            GetComponent<BoxCollider>().enabled = true;
            this.player = player;
            player.onBuildModeChange += OnBuildModeChange;
        }

        public InventoryItem[] GetNeededBuildingElements()
        {
            return elementBuildingRequirements;
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
    }

}