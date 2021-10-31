using Astutos.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        SavingWrapper savingWrapper;

        private void Start()
        {
            savingWrapper = FindObjectOfType<SavingWrapper>();
        }

        public void ContinueLastSave()
        {
            savingWrapper.LoadLastSave();
        }
    }

}