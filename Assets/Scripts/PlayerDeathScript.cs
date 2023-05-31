using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PlayerDeathScript : NetworkBehaviour
{
    public List<GameObject> players;
    public Player localPlayer;
    public CinemachineVirtualCamera mainCamera;
    public bool dead;
    public Spectator spectator;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted1;


        if (!IsOwner)
        {
            enabled = false;

        } else
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }

    }

    private void SceneManager_OnLoadEventCompleted1(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "Level 1")
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            localPlayer = players.ConvertAll(p => p.GetComponent<Player>()).Find(p => p.IsLocalPlayer);
            localPlayer.OnPlayerDeath += LocalPlayer_OnPlayerDeath;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted1;
            spectator = FindObjectOfType<Spectator>();
        }

    }

    public void LocalPlayer_OnPlayerDeath()
    {
        if (Shared.gameMode == GameMode.Coop)
        {
            
            dead = true;

            if (enabled)
                spectator.enabled = true;

            DeactivatePlayerClientRpc(localPlayer.playerID.Value.ToString());
            spectator.SpectateAlive();
        }
        else
        {
            NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        mainCamera = GetComponent<Player>().mainCamera;
        if (!dead)
            return;

        dead = false;
        spectator.enabled = false;

        localPlayer.gameObject.SetActive(true);
        localPlayer.GetComponent<Player>().health = 200;
        mainCamera.Follow = localPlayer.transform;

    }

    [ClientRpc]
    void DeactivatePlayerClientRpc(string playerID)
    {
        Debug.Log("ClientRpc: " + playerID);
        
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i].GetComponent<Player>();
            Debug.Log("Player ID: " + player.OwnerClientId + "/" + player.playerID.Value);
            Debug.Log(player.playerID.Value == playerID);
        }

        GameObject deathPlayer = players.Find(go => go.GetComponent<Player>().playerID.Value == playerID);

        deathPlayer?.SetActive(false);
        
    }
    
}
