using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System.IO;
using Cinemachine;
using UnityEngine.SceneManagement;
using Unity.AI.Navigation;

public class FloorGenerator : NetworkBehaviour
{
    private Seed seed;
    public int floorLevel;
    public List<GameObject> oneDoor;
    public List<GameObject> twoDoorsCorridor;
    public List<GameObject> twoDoorsLShape;
    public List<GameObject> threeDoors;
    public List<GameObject> fourDoors;
    public List<GameObject> especialRoomCandidates;
    public List<GameObject> bossRoomCandidates;

    // Start is called before the first frame update
    void Start()
    {
        seed = FindObjectOfType<Seed>();
        GenerateRooms();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && NetworkManager.Singleton.IsServer)
        {
            GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(p => p.transform.position = Vector3.zero);
            switch (floorLevel)
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
        
        ReplaceRoomList(oneDoor, "1Door");
        especialRoomCandidates = new List<GameObject>(oneDoor);
        ReplaceRoomList(twoDoorsCorridor, "2DoorCorridor");
        ReplaceRoomList(twoDoorsLShape, "2DoorLShape");
        ReplaceRoomList(threeDoors, "3Door");
        ReplaceRoomList(fourDoors, "4Door");
        
        SetItemRooms();
        SelectShopRoom();
        SelectBossRoom();
        Invoke("BuildNavMesh", 0.5f);
        
    }

    void BuildNavMesh()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    private void SelectBossRoom()
    {   
        Transform placeholderRoom = bossRoomCandidates[Shared.criticalRandomGenerator.Next(0, bossRoomCandidates.Count)].transform;
        string path = "Rooms/BossRooms";
        List<GameObject> loadedRooms = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToList();
        loadedRooms.RemoveAll(item => item.name.Contains("Prefab"));
        GameObject randomRoom = loadedRooms[Shared.criticalRandomGenerator.Next(0, loadedRooms.Count)];
        Instantiate(randomRoom, placeholderRoom.position, Quaternion.Euler(0, placeholderRoom.eulerAngles.y, placeholderRoom.eulerAngles.z));
        Destroy(placeholderRoom.gameObject);
         
    }

    void SelectShopRoom()
    {
        int randomPlaceholder = Shared.criticalRandomGenerator.Next(0, especialRoomCandidates.Count);
        Transform placeholderRoom = especialRoomCandidates[randomPlaceholder].transform;

        especialRoomCandidates.Remove(placeholderRoom.gameObject);
        bossRoomCandidates.Remove(placeholderRoom.gameObject);

        string path = "Rooms/ShopRooms";
        List<GameObject> loadedRooms = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToList();
        loadedRooms.RemoveAll(item => item.name.Contains("Prefab"));
        GameObject randomRoom = loadedRooms[Shared.criticalRandomGenerator.Next(0, loadedRooms.Count)];
        Instantiate(randomRoom, placeholderRoom.position, Quaternion.Euler(0, placeholderRoom.eulerAngles.y, placeholderRoom.eulerAngles.z));
        Destroy(placeholderRoom.gameObject);
    }

    void SetItemRooms()
    {
        for (int i = 0; i < /*seed.CountPlayers()*/4 * floorLevel; i++)
        {
            string path = "Rooms/ItemRooms";
            List<GameObject> loadedRooms = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToList();
            loadedRooms.RemoveAll(item => item.name.Contains("Prefab"));

            int random = Shared.criticalRandomGenerator.Next(0, especialRoomCandidates.Count);
            Transform placeholderRoom = especialRoomCandidates[random].transform;

            especialRoomCandidates.Remove(placeholderRoom.gameObject);
            bossRoomCandidates.Remove(placeholderRoom.gameObject);

            random = Shared.criticalRandomGenerator.Next(0, loadedRooms.Count);
            GameObject randomRoom = loadedRooms[random];

            randomRoom = Instantiate(randomRoom, placeholderRoom.position, Quaternion.Euler(0, placeholderRoom.eulerAngles.y, placeholderRoom.eulerAngles.z));
            Destroy(placeholderRoom.gameObject);
        }
    }

    void ReplaceRoomList(List<GameObject> rooms, string roomFolderName)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Transform placeholderRoom = rooms[i].transform;
            string path = "Rooms/" + roomFolderName;
            List<GameObject> loadedRooms = Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>().ToList();
            loadedRooms.RemoveAll(item => item.name.Contains("Prefab"));

            int random = Shared.criticalRandomGenerator.Next(0, loadedRooms.Count);
            GameObject randomRoom = loadedRooms[random];

            randomRoom = Instantiate(randomRoom, placeholderRoom.position, Quaternion.Euler(0,placeholderRoom.eulerAngles.y,placeholderRoom.eulerAngles.z));

            if (rooms[i].GetComponent<RoomPlaceholder>().isBossRoomCandidate)
                bossRoomCandidates.Add(randomRoom);

            Destroy(rooms[i]);
            rooms[i] = randomRoom;
        }
    }
}
