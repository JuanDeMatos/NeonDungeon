using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SharedInventory : NetworkBehaviour
{
    // Consumables
    public NetworkVariable<int> keys = new NetworkVariable<int>(0);
    public delegate void keysModifiedCallback(int n);
    public event keysModifiedCallback OnKeysModified;
    public NetworkVariable<int> coins = new NetworkVariable<int>(0);
    public delegate void coinsModifiedCallback(int n);
    public event keysModifiedCallback OnCoinsModified;

    private void Start()
    {
        DontDestroyOnLoad(this);
        NetworkManager.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        GetComponent<NetworkObject>().Spawn();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (FindObjectsOfType<SharedInventory>().Length > 1)
            Destroy(this.gameObject);
    }

    public void AddKey()
    {
        keys.Value++;
        OnKeysModified(keys.Value);
    }

    public void UseKey()
    {
        keys.Value--;
        OnKeysModified(keys.Value);
    }

    public void AddCoins(int n)
    {
        coins.Value+= n;
        OnCoinsModified(keys.Value);
    }

    public void UseCoins(int n)
    {
        keys.Value-= n;
        OnCoinsModified(keys.Value);
    }
}
