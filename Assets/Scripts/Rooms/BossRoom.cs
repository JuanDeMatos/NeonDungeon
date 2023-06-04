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

    private Seed seed;

    // Start is called before the first frame update
    void Start()
    {
        seed = FindObjectOfType<Seed>();
        if (Shared.gameMode == GameMode.Coop)
        {
            doorTriggers.SetActive(false);
        }
        else
        {
            coopDoorTriggers.gameObject.SetActive(false);
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
        StartCoroutine(LoadNextLevel());

    }

    IEnumerator LoadNextLevel()
    {
        Debug.Log("LoadNextLevel");
        GameObject.FindGameObjectsWithTag("Player").ToList()
                    .ConvertAll<Player>(p => p.GetComponent<Player>())
                    .Find(p => p.IsLocalPlayer)?.StartSpectating();

        if (!NetworkManager.Singleton.IsServer)
            yield break;

        yield return new WaitForSeconds(1f);

        
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

    private void SpawnItems()
    {

        for (int i = 0; i < FindObjectOfType<LooseState>().alivePlayers; i++)
        {
            itemSpots[i].Spawn();

        }
    }

    private void DespawnEnemies()
    {
        GameObject.FindGameObjectsWithTag("Enemy").ToList().ForEach(e => e.GetComponent<NetworkObject>().Despawn(true));
    }
}
