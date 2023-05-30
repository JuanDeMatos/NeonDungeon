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
        NetworkManager.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (FindObjectsOfType<SharedInventory>().Length > 1)
            Destroy(this.gameObject);
    }

    public void AddKeys(int amount)
    {
        if (IsServer)
        {
            keys.Value += amount;
            keys.Value = keys.Value > 99 ? 99 : keys.Value;
        }
        if (OnKeysModified != null) { OnKeysModified(keys.Value); }
    }

    public bool UseKeys(int amount)
    {
        /*
        if (keys.Value - amount < 0)
            return false;
        */
        if (IsServer) {
            keys.Value -= amount; 
        }
        if (OnKeysModified != null) OnKeysModified(keys.Value);

        return true;
    }

    public void AddCoins(int amount)
    {
        if (IsServer) { 
            coins.Value += amount;
            coins.Value = coins.Value > 99 ?99:coins.Value;
        }
        if (OnCoinsModified != null) { OnCoinsModified(coins.Value); }
    }

    public bool UseCoins(int amount)
    {
        /*
        if (coins.Value - amount < 0)
            return false;
        */
        if (IsServer) { coins.Value -= amount; }
        if (OnCoinsModified != null) { OnCoinsModified(coins.Value); }

        return true;
    }
    public int GetKeysAmount()
    {
        return coins.Value;
    }
    public int GetCoinsAmount()
    {
        return coins.Value;
    }
}
