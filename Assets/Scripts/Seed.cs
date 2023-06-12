using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Seed : NetworkBehaviour
{
    public NetworkVariable<int> seed = new NetworkVariable<int>(0);
    public NetworkVariable<int> nPlayers = new NetworkVariable<int>(1);
    public List<GameObject> treasureItems;
    public List<GameObject> shopItems;
    public List<GameObject> permanentTreasureItems;
    public List<GameObject> permanentShopItems;

    // Start is called before the first frame update
    void Start()
    {
        if (Shared.gameMode == GameMode.DailyRun)
            seed.Value = Shared.dailyRunSeed;
        else
            if (NetworkManager.Singleton.IsServer)
                seed.Value = (int) System.DateTime.Now.Ticks;

        treasureItems = Resources.LoadAll("Items/TreasureItems", typeof(GameObject)).Cast<GameObject>().ToList();
        shopItems = Resources.LoadAll("Items/ShopItems", typeof(GameObject)).Cast<GameObject>().ToList();
        permanentTreasureItems = Resources.LoadAll("Items/PermanentTreasureItems", typeof(GameObject)).Cast<GameObject>().ToList();
        permanentShopItems = Resources.LoadAll("Items/PermanentShopItems", typeof(GameObject)).Cast<GameObject>().ToList();

        Shared.criticalRandomGenerator = new System.Random(GetSeed());
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
    }

    private void Singleton_OnClientDisconnectCallback(ulong obj)
    {
        nPlayers.Value--;
        Debug.Log("Players: " + CountPlayers());
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        nPlayers.Value++;
        Debug.Log("Players: " + CountPlayers());
        ApplySeedClientRpc();
    }

    [ClientRpc]
    public void ApplySeedClientRpc()
    {
        Shared.criticalRandomGenerator = new System.Random(GetSeed());
        Random.InitState(GetSeed());
        Debug.Log("Seed applied: " + GetSeed());
    }

    public int GetSeed()
    {
        return seed.Value;
    }

    public int CountPlayers()
    {
        return nPlayers.Value;
    }
}
