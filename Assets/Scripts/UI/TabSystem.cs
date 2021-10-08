using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class TabSystem : MonoBehaviour
    {
        [Tooltip("If null, all children will be taken as tabs")][SerializeField] GameObject[] tabs;
        GameObject oldTab;

        private void Start()
        {
            CloseAllTabs();
        }

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

        private void CloseAllTabs()
        {
            if(tabs == null)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject tab in tabs)
                {
                    tab.SetActive(false);
                }
            }
        }
    }
}