using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] public GameObject roof;
    [SerializeField] protected GameObject doors;
    [SerializeField] protected GameObject doorTriggers;
    public List<GameObject> enemies;
    public bool ended;

    public delegate void RoomStartedHandler();
    public static event RoomStartedHandler OnRoomStarted;
    public delegate void RoomEndHandler();
    public static event RoomEndHandler OnRoomEnd;
    

    public virtual void StartRoom()
    {
        OnRoomStarted();
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
        OnRoomEnd();
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
        if (Shared.gameMode == GameMode.Coop)
            yield break;

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
