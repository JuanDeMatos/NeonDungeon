using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class BossRoomTrigger : RoomDoorTriggers
{   
    [SerializeField] private WaitForPlayers waitForPlayers;

    private void Start()
    {
        this.Invoke(() => active = true, 1f);
        waitForPlayers.OnAllPlayersReady += WaitForPlayers_OnAllPlayersReady;
    }

    private void WaitForPlayers_OnAllPlayersReady()
    {
        room.StartRoom();
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!active)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Entra quitar techo");

        room.roof.SetActive(false);
    }
}
