using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossRoom : Room
{
    [SerializeField] GameObject allPlayersReadyDoors;
    [SerializeField] WaitingPlayersBossRoom waiting;

    // Start is called before the first frame update
    void Start()
    {
        if (Shared.gameMode == GameMode.Coop)
        {
            doors.SetActive(true);
            waiting.OnAllPlayersReady += Waiting_OnAllPlayersReady;
        }
            


    }

    private void Waiting_OnAllPlayersReady()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
