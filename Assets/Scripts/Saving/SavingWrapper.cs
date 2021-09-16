using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astutos.Saving;

namespace CON.Core
{
    public class SavingWrapper : MonoBehaviour
    {
        string saveFile = "testSaveFile";

        SavingSystemEncrypted savingSystemEncrypted;

        private void Awake()
        {
            savingSystemEncrypted = GetComponent<SavingSystemEncrypted>();
            savingSystemEncrypted.Load(saveFile);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                savingSystemEncrypted.Save(saveFile);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                savingSystemEncrypted.Load(saveFile);
            }
        }
    }
}
