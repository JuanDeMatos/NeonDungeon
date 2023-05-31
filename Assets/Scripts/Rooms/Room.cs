using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] protected GameObject roof;
    [SerializeField] protected GameObject doors;
    [SerializeField] protected GameObject doorTriggers;
    public List<GameObject> enemies;
    public bool ended;

    public virtual void StartRoom()
    {
        roof.SetActive(false);
        if (Shared.gameMode == GameMode.Singleplayer)
        {
            Shared.inCombat = true;
            doors.SetActive(true);
        }
        doorTriggers.SetActive(false);
        StartCoroutine(SpawnObjects());
    }

    public virtual void EndRoom()
    {
        doors.SetActive(false);
        Shared.inCombat = false;
    }

    protected virtual IEnumerator SpawnObjects()
    {

        List<ObjectSpawner> spawners = GetComponentsInChildren<ObjectSpawner>().ToList();

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].Spawn();
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(CheckEnemies());
    }

    protected virtual IEnumerator CheckEnemies()
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
}
