using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRoomDoorTrigger : MonoBehaviour
{

    [SerializeField] private ItemRoom room;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Shared.inCombat && Shared.gameMode == GameMode.Singleplayer)
            return;

        SharedInventory sharedInventory = FindObjectOfType<SharedInventory>();
        /*
        if (sharedInventory.GetKeysAmount() <= 0)
            return;
        */
        sharedInventory.UseKeys(1);

        room.StartRoom();

    }
}
