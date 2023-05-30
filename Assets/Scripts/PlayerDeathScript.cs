using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerDeathScript : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public List<GameObject> alivePlayers;
    public int lookingAtPlayer = 0;
    public Player localPlayer;
    public Player playerInventory;
    public CinemachineVirtualCamera mainCamera;
    public bool dead;

    // Start is called before the first frame update
    void Start()
    {
        alivePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        localPlayer = alivePlayers.ConvertAll(p => p.GetComponent<Player>()).Find(p => p.IsLocalPlayer);
        localPlayer.OnPlayerDeath += LocalPlayer_OnPlayerDeath;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!dead)
            return;

        dead = false;
        RespawnServerRpc();
    }

    [ServerRpc]
    void RespawnServerRpc()
    {
        playerInventory.health = 200;
        GameObject copy = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        Player respawnedPlayer = copy.GetComponent<Player>();
        respawnedPlayer.SetAllStats(playerInventory);
        copy.GetComponent<NetworkObject>().Spawn();
        
    }

    void Update()
    {
        alivePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

    private void LocalPlayer_OnPlayerDeath()
    {
        playerInventory = localPlayer.Clone();
        Debug.Log("Player death");
        if (Shared.gameMode == GameMode.Coop)
        {
            dead = true;
            mainCamera.Follow = alivePlayers[0].transform;
            StartCoroutine(Spectator());
        } else
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
        
    }

    IEnumerator Spectator()
    {
        while (true)
        {
            Debug.Log("Spectator");
            if (Input.GetKey(KeyCode.RightArrow))
            {
                lookingAtPlayer++;
                if (lookingAtPlayer >= alivePlayers.Count)
                    lookingAtPlayer = 0;

                mainCamera.Follow = alivePlayers[lookingAtPlayer].transform;
                yield return new WaitForSeconds(0.2f);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                lookingAtPlayer--;
                if (lookingAtPlayer < 0)
                    lookingAtPlayer = alivePlayers.Count - 1;

                mainCamera.Follow = alivePlayers[lookingAtPlayer].transform;
                yield return new WaitForSeconds(0.2f);
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
    
}
