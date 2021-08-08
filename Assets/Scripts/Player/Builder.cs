using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Player
{
    public class Builder : MonoBehaviour
    {
        [SerializeField] Transform buildObjectsParent;

        Grid grid;
        bool buildMode = false;
        GameObject currentMachine;

        private void Awake()
        {
            grid = new Grid(200, 150, 1.5f, new Vector3(-90, 0.4f, 0));
        }

        
        private void Update()
        {
            if (buildMode)
            {
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit))
                {
                    int x;
                    int y;
                    grid.GetGridPosition(raycastHit.point, out x, out y);

                    Machine machine = currentMachine.transform.GetComponent<Machine>();
                    if (grid.IsObstructed(x, y) || machine != null && !grid.HasElement(x, y, machine.GetElementRequirement())) return;

                    currentMachine.transform.position = grid.GetNearestGridWorldPosition(grid.GetWorldPosition(x,y));

                    if (Input.GetMouseButtonDown(0))
                    {
                        grid.GetGridPosition(raycastHit.point, out x, out y);

                        IPlaceable currentMachinePlaceable = currentMachine.GetComponent<IPlaceable>();
                        foreach (Vector2Int takenGridPosition in currentMachinePlaceable.GetTakenGridPositions())
                        {
                            grid.SetValue(x + takenGridPosition.x, y + takenGridPosition.y,true);
                        }

                        currentMachinePlaceable.FullyPlaced();

                        buildMode = false;
                        currentMachine = null;
                        return;
                    }
                }
            }
        }

        public void ActivateBuildMode(GameObject machine)
        {
            if (currentMachine != null) Destroy(currentMachine);
            buildMode = true;
            currentMachine = Instantiate(machine);
            currentMachine.transform.parent = buildObjectsParent;
        }
    }

}