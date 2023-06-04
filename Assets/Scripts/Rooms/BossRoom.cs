using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

public class BossRoom : Room
{
    [SerializeField] private BossRoomTrigger coopDoorTriggers;
    [SerializeField] private List<ItemSpawner> itemSpots;
    [SerializeField] private WaitForPlayers waitForNextLevel;

    // Start is called before the first frame update
    void Start()
    {
        if (Shared.gameMode == GameMode.Coop)
        {
            doorTriggers.SetActive(false);
        }
        else
        {
            coopDoorTriggers.gameObject.SetActive(false);
        }

        if (NetworkManager.Singleton.IsServer)
            waitForNextLevel.OnAllPlayersReady += LoadNextLevel;
    }

    void LoadNextLevel()
    {

        switch (FindObjectOfType<FloorGenerator>().floorLevel)
        {
            case 1:
                NetworkManager.Singleton.SceneManager.LoadScene("Level 2", LoadSceneMode.Single);
                break;
            case 2:
                NetworkManager.Singleton.SceneManager.LoadScene("Level 3", LoadSceneMode.Single);
                break;
            case 3:
                NetworkManager.Singleton.SceneManager.LoadScene("Level 3", LoadSceneMode.Single);
                break;
        }
    }

    public override void StartRoom()
    {
        roof.SetActive(false);
        Shared.inCombat = true;
        doors.SetActive(true);
        doorTriggers.SetActive(false);
        coopDoorTriggers.gameObject.SetActive(false);
        StartCoroutine(SpawnObjects());
    }

    protected override IEnumerator SpawnObjects()
    {
        if (NetworkManager.Singleton.IsServer)
            DespawnEnemies();

        return base.SpawnObjects();
    }
    protected override IEnumerator CheckEnemies()
    {
        yield return new WaitForSeconds(2f);

        do
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

            if (enemies.Count == 0)
                ended = true;
            else
                yield return new WaitForSeconds(0.1f);

        } while (!ended);

        EndRoom();
    }

    public override void EndRoom()
    {
        base.EndRoom();
        SpawnItems();
        waitForNextLevel.gameObject.SetActive(true);
    }

    private void SpawnItems()
    {

        for (int i = 0; i < LooseState.alivePlayers; i++)
        {
            itemSpots[i].Spawn();

        }
    }

    private void DespawnEnemies()
    {
        GameObject.FindGameObjectsWithTag("Enemy").ToList().ForEach(e => e.GetComponent<NetworkObject>().Despawn(true));
    }
}
