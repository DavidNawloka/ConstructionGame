using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class TabSystem : MonoBehaviour
    {
        GameObject oldTab;
        
        public void CloseTab()
        {
            if (oldTab != null) oldTab.SetActive(false);
            oldTab = null;
            return;
        }
        public void ShowTab(GameObject newTab)
        {
            if (oldTab != null) oldTab.SetActive(false);

            oldTab = newTab;
            newTab.SetActive(true);
        }
    }

}