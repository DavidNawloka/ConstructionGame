using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace CON.Core
{
    public class ControlMapping : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buttonName;
        [SerializeField] Image buttonImage;


        SettingsManager settingsManager;

        private void Awake()
        {
            
            settingsManager = GetComponentInParent<SettingsManager>();
        }

        public void InitialiseControlMapping(string buttonName, Sprite buttonImage)
        {
            this.buttonName.text = buttonName;
            this.buttonImage.sprite = buttonImage;
        }

        public void OnClick()
        {
            settingsManager.StartListeningToInput(buttonName.text);
        }
    }

}