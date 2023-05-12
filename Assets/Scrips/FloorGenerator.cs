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

        Debug.Log(Shared.joinCode);

        StartCoroutine(WaitForPlayers());

        Debug.Log("Seed: " + seed.Value);
        Random.InitState(seed.Value);

        GenerateRooms();
        StartCoroutine(InitializePlayers());

    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsServer && Input.GetKeyDown(KeyCode.K))
        {
            GameObject copy = Instantiate(enemies[0], new Vector3(0,1,0), Quaternion.identity);
            copy.GetComponent<NetworkObject>().Spawn();
        }

    }

    IEnumerator WaitForPlayers()
    {
        int nPlayers = 0;
        do
        {
            nPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
            yield return new WaitForSeconds(0.1f);
        } while (nPlayers < 2);

    }

    IEnumerator InitializePlayers()
    {   
        GameObject player = GameObject.FindGameObjectsWithTag("Player").ToList().Find(go => go.GetComponent<Player>().IsOwner);

        player.GetComponent<Player>().usesGravity = false;
        player.transform.position = Vector3.zero;
        player.GetComponent<Player>().movement = Vector3.zero;

        GameObject camera = null;
        do
        {
            camera = GameObject.Find("CM vcam1");
            if (camera != null)
            {
                camera.GetComponent<CinemachineVirtualCamera>().m_Follow = player.transform;
            }
            yield return new WaitForEndOfFrame();
        } while (camera == null);

        yield return new WaitForSeconds(1);
        player.GetComponent<Player>().usesGravity = true;
    }

    void GenerateRooms()
    {

        Debug.Log("Entra");
        ReplaceRoomList(oneDoor, "1Door");
        ReplaceRoomList(twoDoorsCorridor, "2DoorCorridor");
        ReplaceRoomList(twoDoorsLShape, "2DoorLShape");
        ReplaceRoomList(threeDoors, "3Door");
        ReplaceRoomList(fourDoors, "4Door");

        StartCoroutine(SpawnEnemies());
    }

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

    void ReplaceRoomList(List<GameObject> rooms, string roomFolderName)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Transform placeholderRoom = rooms[i].transform;
            List<string> roomIdentifierList = Directory.GetFiles("./Assets/Resources/Rooms/" + roomFolderName).ToList();
            roomIdentifierList.RemoveAll(item => item.Contains(".meta") || item.Contains("Prefab"));
            string roomIdentifier = Path.GetFileName(roomIdentifierList[Random.Range(0, roomIdentifierList.Count)]).Split(".")[0];
            string ruta = "Rooms/" + roomFolderName + "/" + roomIdentifier;
            GameObject randomRoom = Resources.Load<GameObject>(ruta);
            randomRoom = Instantiate(randomRoom, placeholderRoom.position, Quaternion.Euler(0,placeholderRoom.eulerAngles.y,placeholderRoom.eulerAngles.z));
            Destroy(rooms[i]);
            rooms[i] = randomRoom;
        }
    }
}
