using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class LooseState : NetworkBehaviour
{
    public List<GameObject> players;
    public static int alivePlayers = 1;
    public Player localPlayer;

    public delegate void ShutdownHandler();
    public static event ShutdownHandler OnShutdown;

    private void Start()
    {
        StartCoroutine(WaitingLobby());
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKeyDown(KeyCode.Escape))
        {
            Shutdown(false);
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {

        if (sceneName == "Level 1")
        {
            StopAllCoroutines();
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            localPlayer = players.ConvertAll(p => p.GetComponent<Player>()).Find(p => p.IsLocalPlayer);
            localPlayer.OnPlayerDeath += LocalPlayer_OnPlayerDeath;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
            StartCoroutine(CheckAlivePlayers());
        }
    }

    IEnumerator WaitingLobby()
    {
        while (true)
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            StartCoroutine(CheckAlivePlayers());
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void LocalPlayer_OnPlayerDeath()
    {
        if (Shared.gameMode == GameMode.Singleplayer)
        {
            Shutdown(false);
        }
    }

    IEnumerator CheckAlivePlayers()
    {
        while (true)
        {
            players.RemoveAll(go => go == null);
            alivePlayers = players.ConvertAll(go => go.GetComponent<PlayerDeathScript>()).FindAll(p => !p.dead).Count;

            if (players.Count == 0 || alivePlayers <= 0)
            {
                Shutdown(false);
            }

            yield return new WaitForSeconds(1);
        }
    }

    public void Shutdown(bool win)
    {
        if (IsServer)
            ShutdownClientRpc(win);
        else
        {
            RunScore.win = win;
            OnShutdown();
            NetworkManager.Singleton.Shutdown();
            Destroy(GameObject.Find("NetworkManager"));
            SceneManager.LoadScene("GameOver");
        }
    }

    [ClientRpc]
    private void ShutdownClientRpc(bool win)
    {
        RunScore.win = win;
        OnShutdown();
        NetworkManager.Singleton.Shutdown();
        Destroy(GameObject.Find("NetworkManager"));
        SceneManager.LoadScene("GameOver");
    }
}
