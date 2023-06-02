using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossRoom : Room
{
    [SerializeField] GameObject allPlayersReadyDoors;
    [SerializeField] GameObject playersText;
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

    public override void StartRoom()
    {
        roof.SetActive(false);
        Shared.inCombat = true;
        doors.SetActive(true);
        doorTriggers.SetActive(false);
        StartCoroutine(SpawnObjects());
    }

    private void Waiting_OnAllPlayersReady()
    {
        allPlayersReadyDoors.SetActive(true);
        doors.SetActive(false);
        playersText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
