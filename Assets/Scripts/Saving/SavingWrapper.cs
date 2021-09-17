using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astutos.Saving;

namespace CON.Core
{
    public class SavingWrapper : MonoBehaviour, ISaveable
    {
        string saveFileName = "testSaveFile";
        SavingSystemEncrypted savingSystemEncrypted;

        

        private void Awake()
        {
            savingSystemEncrypted = GetComponent<SavingSystemEncrypted>();
            savingSystemEncrypted.Load(saveFileName);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                savingSystemEncrypted.Save(saveFileName);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                savingSystemEncrypted.Load(saveFileName);
            }
        }

        public object CaptureState()
        {
            return 2;
        }

        public void RestoreState(object state)
        {
        }
    }
}
