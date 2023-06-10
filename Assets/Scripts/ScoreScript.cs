using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    private bool started;
    private float levelMultiplier = 1;

    private void Start()
    {
        Enemy.OnDeath += Enemy_OnDeath;
        Player.OnHit += Player_OnHit;
        if (Shared.gameMode != GameMode.Coop)
            Room.OnRoomEnd += Room_OnRoomEnd;
        FloorGenerator.OnLevelLoaded += FloorGenerator_OnLevelLoaded;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void Player_OnHit()
    {
        RunScore.hits++;
        RunScore.score -= 200;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "Level 1")
        {
            RunScore.time = 0;
            RunScore.hits = 0;
            RunScore.score = 10000;
            started = true;
        } else if (sceneName == "GameOver")
        {
            started = false;
        }
    }

    private void FloorGenerator_OnLevelLoaded()
    {
        levelMultiplier += 0.5f;
    }

    private void Room_OnRoomEnd()
    {
        RunScore.score += 200 * levelMultiplier;
    }

    private void Enemy_OnDeath(float points)
    {
        RunScore.score += points * levelMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
            return;

        RunScore.score -= Time.deltaTime * 1.3f;
        RunScore.time += Time.deltaTime;

    }
}
