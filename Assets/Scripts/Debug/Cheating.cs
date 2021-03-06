using CON.Elements;
using CON.Progression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheating : MonoBehaviour
{
    [SerializeField] Unlockable[] placeableToUnlock;
    [SerializeField] FindableNoteIdentifier[] notesToUnlock;

    bool f2Pressed = false;
    bool f3Pressed = false;

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
            if (!f2Pressed && Input.GetKeyDown(KeyCode.F2))
            {
                f2Pressed = true;
                foreach(Unlockable placeable in placeableToUnlock)
                {
                    FindObjectOfType<ProgressionManager>().UnlockPlaceable(placeable);
                }
            }
            if (!f3Pressed && Input.GetKeyDown(KeyCode.F3))
            {
                f3Pressed = true;
                foreach (FindableNoteIdentifier findableNote in notesToUnlock)
                {
                    FindObjectOfType<FindableNoteManager>().EquipNewNote(findableNote,Vector2.zero);
                }
            }
        }
    }
}
