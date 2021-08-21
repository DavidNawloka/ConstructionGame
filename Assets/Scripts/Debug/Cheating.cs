using CON.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheating : MonoBehaviour
{
    void Update()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Element wood = (Element)Resources.Load("Wood");
                Element rock = (Element)Resources.Load("Rock");
                Element water = (Element)Resources.Load("Water");
                GetComponent<Inventory>().EquipItem(new InventoryItem(wood, 100));
                GetComponent<Inventory>().EquipItem(new InventoryItem(rock, 100));
                GetComponent<Inventory>().EquipItem(new InventoryItem(water, 100));
            }
        }
    }
}
