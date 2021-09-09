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
                Element crystal = (Element)Resources.Load("Crystal");
                GetComponent<Inventory>().EquipItem(new InventoryItem(crystal, 100));
                GetComponent<Inventory>().EquipItem(new InventoryItem(wood, 100));
                GetComponent<Inventory>().EquipItem(new InventoryItem(rock, 100));
                GetComponent<Inventory>().EquipItem(new InventoryItem(water, 100));
            }
        }
    }
}
