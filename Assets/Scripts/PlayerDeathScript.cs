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

    public override void OnNetworkSpawn()
    {

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted1;
        if (!IsOwner)
        {
            enabled = false;

        } else
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_Respawn;
        }

    }

    private void SceneManager_OnLoadEventCompleted1(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "Level 1")
        {
            Debug.Log("Entra en onload");
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
            if (enabled)
                spectator.enabled = true;
           

            spectator.SpectateAlivePlayer();
        }
    }

    private void SceneManager_Respawn(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "WaitingLobby")
            return;

        mainCamera = GetComponent<Player>().mainCamera;
        spectator.Disable();

        players.ForEach((go) =>
        {
            Player player = go.GetComponent<Player>();

            player.Heal((int)player.maxHealth / 3);

            go.SetActive(true);
            player.StartApplyPlayerProperties();

        });
        
        mainCamera.Follow = localPlayer.transform;

    }

    [ServerRpc]
    public void DeactivatePlayerServerRpc(ulong ownerClientId)
    {
        Debug.Log("ServerRpc: " + ownerClientId);
        DeactivatePlayerClientRpc(ownerClientId);
    }

    [ClientRpc]
    void DeactivatePlayerClientRpc(ulong ownerClientId)
    {
        Debug.Log("ClientRpc: " + ownerClientId);
        
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i].GetComponent<Player>();
        }

        GameObject deathPlayer = players.Find(go => go.GetComponent<Player>().OwnerClientId == ownerClientId);

        deathPlayer.SetActive(false);  
    }
    
}
