using CON.Core;
using CON.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Machines
{
    public class Conveyor : MonoBehaviour, IPlaceable
    {
        [SerializeField] PlaceableInformation placeableInformation;
        [SerializeField] Transform[] pathOfElement;
        [SerializeField] float forceToApplyForward;
        [SerializeField] float forceToApplySide;
        [SerializeField] GameObject directionArrow;
        [SerializeField] AudioClip[] conveyorSounds;

        Builder player;
        AudioSourceManager audioLoop;

        private void Awake()
        {
            audioLoop = GetComponent<AudioSourceManager>();
        }

        private void Start()
        {
            if (player != null) OnBuildModeChange(false);
        }
        private void OnDisable()
        {
            if (player == null) return;
            player.onBuildModeChange -= OnBuildModeChange;
        }

        private void OnCollisionStay(Collision collision)
        {
            ElementPickup elementPickup = collision.transform.GetComponentInParent<ElementPickup>();
            if (elementPickup == null) return;

            Rigidbody rigidbody = collision.transform.GetComponentInParent<Rigidbody>();

            rigidbody.velocity = (pathOfElement[1].position -pathOfElement[0].position).normalized * forceToApplyForward;
            rigidbody.AddForce(GetForceToKeepOnConveyor(rigidbody.transform)* forceToApplySide, ForceMode.Impulse);
        }

        private Vector3 GetForceToKeepOnConveyor(Transform element)
        {
            if(Mathf.Approximately(transform.rotation.eulerAngles.y,90) || Mathf.Approximately(transform.rotation.eulerAngles.y, 270))
            {
                return new Vector3(transform.position.x - element.position.x, 0, 0);
            }
            else
            {
                return new Vector3(0, 0, transform.position.z - element.position.z);
            }
        }
        private void OnBuildModeChange(bool isActive)
        {
            directionArrow.SetActive(isActive);
        }

        // Interface implementations

        public void StartingPlacement(Builder player)
        {
            player.onBuildModeChange += OnBuildModeChange;
            this.player = player;
        }
        public void FullyPlaced(Builder player)
        {
            GetComponent<BoxCollider>().enabled = true;
            audioLoop.StartLooping(conveyorSounds);
        }
        public void ChangeVersion()
        {
            
        }
        public void ChangeColor(Color color)
        {
            placeableInformation.normalPlaceable.SetActive(false);
            placeableInformation.greenPlaceable.SetActive(false);
            placeableInformation.redPlaceable.SetActive(false);

            if (color == Color.green) placeableInformation.greenPlaceable.SetActive(true);
            else if (color == Color.red) placeableInformation.redPlaceable.SetActive(true);
            else placeableInformation.normalPlaceable.SetActive(true);
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public PlaceableInformation GetPlaceableInformation()
        {
            return placeableInformation;
        }
        public object GetInformationToSave()
        {
            return null;
        }

        public void LoadSavedInformation(object savedInformation)
        {

        }
    }

}