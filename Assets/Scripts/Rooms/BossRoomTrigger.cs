using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class BossRoomTrigger : RoomDoorTriggers
{   
    [SerializeField] private TextMeshPro playersReadyText;
    //public NetworkVariable<int> numberPlayersReady;
    public int numberPlayersReady = 0;

    private LooseState looseState;

    void Update()
    {
        playersReadyText.transform.eulerAngles = new Vector3(60, 0, 0);
        if (looseState == null)
            looseState = FindObjectOfType<LooseState>();
        else
        {
            playersReadyText.SetText(numberPlayersReady + " / " + looseState.alivePlayers);

            if (numberPlayersReady == looseState.alivePlayers)
            {
                room.StartRoom();
                this.gameObject.SetActive(false);
            } 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!active)
            return;

        if (!other.CompareTag("Player"))
            return;

        Debug.Log(other.gameObject.name);

        room.roof.SetActive(false);
        AddPlayerServerRpc();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!active)
            return;

        if (!other.CompareTag("Player"))
            return;

        RemovePlayerServerRpc();

    }

    [ServerRpc]
    void AddPlayerServerRpc()
    {
        numberPlayersReady++;
    }

    [ServerRpc]
    void RemovePlayerServerRpc()
    {
        numberPlayersReady--;
    }
}
