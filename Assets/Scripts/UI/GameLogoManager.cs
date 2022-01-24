using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class GameLogoManager : MonoBehaviour
    {
        bool hasPlayed = false;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !hasPlayed)
            {
                GetComponent<Animation>().Play();
                hasPlayed = true;
            }
        }
    }

}