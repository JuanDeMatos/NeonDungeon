using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyDontDestroyOnLoad : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        LooseState.OnShutdown += LooseState_OnShutdown;
    }

    private void LooseState_OnShutdown()
    {
        Destroy(this.gameObject);
        LooseState.OnShutdown -= LooseState_OnShutdown;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainMenu" || arg0.name == "GameOver")
        {
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            Destroy(this.gameObject);
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "MainMenu" || sceneName == "GameOver")
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
            Destroy(this.gameObject);
        }
    }

}
