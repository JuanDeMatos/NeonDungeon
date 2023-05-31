using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LooseState : MonoBehaviour
{
    public List<GameObject> players;
    public Player localPlayer;

    private void Start()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.Escape))
            ShutdownServerRpc();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "Level 1")
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            localPlayer = players.ConvertAll(p => p.GetComponent<Player>()).Find(p => p.IsLocalPlayer);
            localPlayer.OnPlayerDeath += LocalPlayer_OnPlayerDeath;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
            StartCoroutine(CheckAlivePlayers());
        }
    }

    private void LocalPlayer_OnPlayerDeath()
    {
        if (Shared.gameMode == GameMode.Singleplayer)
        {
            ShutdownServerRpc();
        }
    }

    IEnumerator CheckAlivePlayers()
    {
        while (true)
        {
            if (players.Count == 0 || players.FindAll(go => go.activeSelf).Count <= 0)
                ShutdownServerRpc();
            yield return new WaitForSeconds(1);
        }
    }

    [ServerRpc]
    public void ShutdownServerRpc()
    {
        ShutdownClientRpc();
    }

    [ClientRpc]
    public void ShutdownClientRpc()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
        SceneManager.LoadScene("MainMenu");
    }
    
}
