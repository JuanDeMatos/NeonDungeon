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
        Debug.Log("Entra OnShutdown");
        Destroy(this.gameObject);
        LooseState.OnShutdown -= LooseState_OnShutdown;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "MainMenu")
        {
            Debug.Log("Entra Main Menu");
            Destroy(this.gameObject);
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == "MainMenu")
        {
            Destroy(this.gameObject);
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted; 
        }
    }
}
