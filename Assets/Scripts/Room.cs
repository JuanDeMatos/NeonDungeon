using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject roof;
    [SerializeField] private GameObject doors;
    [SerializeField] private GameObject doorTriggers;
    public List<GameObject> enemies;
    public bool ended;

    public void StartRoom()
    {
        roof.SetActive(false);
        if (Shared.gameMode == GameMode.Singleplayer)
        {
            doors.SetActive(true);
        }
        doorTriggers.SetActive(false);
        Shared.inCombat = true;
        StartCoroutine(SpawnObjects());
    }

    public void EndRoom()
    {
        doors.SetActive(false);
        Shared.inCombat = false;
    }

    IEnumerator SpawnObjects()
    {

        List<ObjectSpawner> spawners = GetComponentsInChildren<ObjectSpawner>().ToList();

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].Spawn();
            yield return new WaitForSeconds(0.1f);
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        StartCoroutine(CheckEnemies());
    }

    IEnumerator CheckEnemies()
    {
        do
        {
            enemies.RemoveAll(e => e == null);

            if (enemies.Count == 0)
                ended = true;
            else
                yield return new WaitForSeconds(0.1f);

        } while (!ended);

        EndRoom();
    }
}
