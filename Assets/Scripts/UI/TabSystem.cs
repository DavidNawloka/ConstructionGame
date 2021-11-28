using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class TabSystem : MonoBehaviour
    {
        [Tooltip("If null, no tab will be shown")] [SerializeField] GameObject defaultTabToShow;
        [Tooltip("If null, all children will be taken as tabs")][SerializeField] GameObject[] tabs;
        [SerializeField] bool areCanvasGroups = false;


        GameObject oldTab;

        private void Start()
        {
            CloseAllTabs();
            if (defaultTabToShow != null) ShowTab(defaultTabToShow);
        }

        public void CloseTab()
        {
            if (oldTab != null)
            {
                if (areCanvasGroups) SetActiveCanvasGroup(oldTab.GetComponent<CanvasGroup>(), false);
                else oldTab.SetActive(false);
            }
            oldTab = null;
            return;
        }
        public void ShowTab(GameObject newTab)
        {
            if (newTab == oldTab) return;
            if (oldTab != null) CloseTab();

            if (areCanvasGroups) SetActiveCanvasGroup(newTab.GetComponent<CanvasGroup>(), true);
            else newTab.SetActive(true);
            oldTab = newTab;
            
        }

        private void SetActiveCanvasGroup(CanvasGroup canvasGroup, bool isActive)
        {
            if (isActive) canvasGroup.alpha = 1;
            else canvasGroup.alpha = 0;

            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }

        private void CloseAllTabs()
        {
            if(tabs == null)
            {
                foreach (Transform child in transform)
                {
                    if (areCanvasGroups) SetActiveCanvasGroup(child.GetComponent<CanvasGroup>(), false);
                    else child.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (GameObject tab in tabs)
                {
                    if (areCanvasGroups) SetActiveCanvasGroup(tab.GetComponent<CanvasGroup>(), false);
                    else tab.SetActive(false);
                }
            }
        }
    }
}