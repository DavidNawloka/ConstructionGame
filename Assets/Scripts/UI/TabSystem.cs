using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.UI
{
    public class TabSystem : MonoBehaviour
    {
        [Tooltip("If null, no tab will be shown")] [SerializeField] GameObject defaultTabToShow;
        [Tooltip("If null, all children will be taken as tabs")][SerializeField] GameObject[] tabs;
        [Tooltip("If false tab will appear instantly")][SerializeField] bool shouldSlideInFromLeft = false;
        [Tooltip("Can be ignored if bool above false")][SerializeField] float timeToShowTab;


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
                oldTab.SetActive(false);
                if (shouldSlideInFromLeft) oldTab.transform.localScale = Vector3.zero;
            }
            oldTab = null;
            return;
        }
        public void ShowTab(GameObject newTab)
        {
            if (newTab == oldTab) return;
            if (oldTab != null) CloseTab();


            if (!shouldSlideInFromLeft)
            {
                newTab.transform.localScale = Vector3.one;
                newTab.SetActive(true);
            }
            else StartCoroutine(SlideLeft(newTab.transform));
            oldTab = newTab;
            
        }

        private void CloseAllTabs()
        {
            if(tabs == null)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                    if (shouldSlideInFromLeft) child.localScale = Vector3.zero;
                }
            }
            else
            {
                foreach (GameObject tab in tabs)
                {
                    tab.SetActive(false);
                    if (shouldSlideInFromLeft) tab.transform.localScale = Vector3.zero;
                }
            }
        }

        private IEnumerator SlideLeft(Transform tab)
        {
            float timer = 0;

            tab.gameObject.SetActive(true);
            while (timer < timeToShowTab)
            {
                tab.localScale = new Vector3(timer / timeToShowTab, 1, 1);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            tab.localScale = Vector3.one;
        }
    }
}