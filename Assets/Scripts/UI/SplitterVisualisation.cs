using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CON.UI
{
    public class SplitterVisualisation : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown ratioDropdown;
        [SerializeField] Toggle switchHookToggle;
        [SerializeField] Image sideArrow;

        MoveableWindow moveableWindow;
        SplitterConveyor splitterConveyor;

        private void Awake()
        {
            moveableWindow = GetComponent<MoveableWindow>();
            splitterConveyor = GetComponentInParent<SplitterConveyor>();
            splitterConveyor.OnSplitterClicked += MachineClicked;
            splitterConveyor.OnFullyPlaced += FullyPlaced;
            ratioDropdown.value = 1;
            ratioDropdown.RefreshShownValue();
        }
        private void FullyPlaced()
        {
            switchHookToggle.isOn = splitterConveyor.isRightToLeft;
        }
        public void MachineClicked()
        {
            transform.position = Camera.main.WorldToScreenPoint(splitterConveyor.transform.position) + new Vector3(Screen.width * .2f, Screen.height * .2f, 0);
            moveableWindow.ToggleCanvas(splitterConveyor.transform);
        }
        public void SwitchHookToggle(bool isRightToLeft)
        {
            splitterConveyor.isRightToLeft = isRightToLeft;
            splitterConveyor.UpdateHookPosition();
            if (!isRightToLeft) sideArrow.transform.rotation = Quaternion.Euler(0, 0, 180);
            else sideArrow.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        public void SwitchRatio(int afterHowManyElements)
        {
            splitterConveyor.UpdateElementRatio(afterHowManyElements);
        }
    }

}