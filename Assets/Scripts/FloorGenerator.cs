using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System.IO;
using Cinemachine;
using UnityEngine.SceneManagement;

public class FloorGenerator : NetworkBehaviour
{
    private NetworkVariable<int> seed = new NetworkVariable<int>(0);
    //public List<Transform> enemyPositions;
    public List<GameObject> enemies;
    public List<GameObject> oneDoor;
    public List<GameObject> twoDoorsCorridor;
    public List<GameObject> twoDoorsLShape;
    public List<GameObject> threeDoors;
    public List<GameObject> fourDoors;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            seed.Value = (int)System.DateTime.Now.Ticks;
        }

        /*
        if (IsServer)
            StartCoroutine(WaitForPlayers());
        */
        Debug.Log("Seed: " + seed.Value);
        Random.InitState(seed.Value);

        GenerateRooms();
        //StartCoroutine(InitializePlayers());


    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (IsServer && Input.GetKeyDown(KeyCode.K))
        {
            GameObject copy = Instantiate(enemies[0], new Vector3(0,1,0), Quaternion.identity);
            copy.GetComponent<NetworkObject>().Spawn();
        }
        */
    }

    IEnumerator WaitForPlayers()
    {
        int nPlayers = 0;
        do
        {
            nPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
            yield return new WaitForSeconds(0.1f);
        } while (nPlayers < NetworkManager.ConnectedClientsList.Count);
        StartCoroutine(InitializePlayers());
    }

    IEnumerator InitializePlayers()
    {   
        GameObject player = GameObject.FindGameObjectsWithTag("Player").ToList().Find(go => go.GetComponent<Player>().IsOwner);

        player.GetComponent<Player>().usesGravity = false;
        player.transform.position = Vector3.zero;
        player.GetComponent<Player>().movement = Vector3.zero;

        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Player>().usesGravity = true;
    }

    void GenerateRooms()
    {

        Debug.Log("Entra Generate Rooms");
        ReplaceRoomList(oneDoor, "1Door");
        ReplaceRoomList(twoDoorsCorridor, "2DoorCorridor");
        ReplaceRoomList(twoDoorsLShape, "2DoorLShape");
        ReplaceRoomList(threeDoors, "3Door");
        ReplaceRoomList(fourDoors, "4Door");

        //StartCoroutine(SpawnEnemies());
        //StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        List<ObjectSpawner> spawners = FindObjectsOfType<ObjectSpawner>().ToList();

        for (int i = 0; i < spawners.Count; i++)
        {
            spawners[i].Spawn();
            yield return new WaitForSeconds(0.2f);
        }
    }

    /*
    IEnumerator SpawnEnemies()
    {
        if (IsServer)
        {
            EnemyPosition[] enemyPositions = FindObjectsOfType<EnemyPosition>();

            foreach (EnemyPosition ep in enemyPositions)
            {
                GameObject enemyPrefab = enemies.Find(e => e.name.Contains(ep.enemyType.ToString()));
                GameObject copy = Instantiate(enemyPrefab, ep.transform.position, ep.transform.rotation);
                copy.GetComponent<NetworkObject>().Spawn();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            GameObject.FindGameObjectsWithTag("EnemyPosition").ToList().ForEach(position =>
            {
                Destroy(position);
            });
            
        }
    }

    */

    void ReplaceRoomList(List<GameObject> rooms, string roomFolderName)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Transform placeholderRoom = rooms[i].transform;
            string ruta = "Rooms/" + roomFolderName;
            List<GameObject> loadedRooms = Resources.LoadAll(ruta, typeof(GameObject)).Cast<GameObject>().ToList();
            loadedRooms.RemoveAll(item => item.name.Contains("Prefab"));
            GameObject randomRoom = loadedRooms[Random.Range(0, loadedRooms.Count)];
            randomRoom = Instantiate(randomRoom, placeholderRoom.position, Quaternion.Euler(0,placeholderRoom.eulerAngles.y,placeholderRoom.eulerAngles.z));
            Destroy(rooms[i]);
            rooms[i] = randomRoom;
        }
    }
}
