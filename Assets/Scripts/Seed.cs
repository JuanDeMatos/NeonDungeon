using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Seed : NetworkBehaviour
{
    public NetworkVariable<int> seed = new NetworkVariable<int>(0);
    public NetworkVariable<int> nPlayers = new NetworkVariable<int>(1);
    public List<GameObject> availableItems;
    public GameObject lastItem;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        if (NetworkManager.Singleton.IsServer)
            seed.Value = (int) System.DateTime.Now.Ticks;
            //seed.Value = 5000;

        availableItems = Resources.LoadAll("Items", typeof(GameObject)).Cast<GameObject>().ToList();
        availableItems.RemoveAll(i => i.name == "LastItem");
        lastItem = Resources.Load<GameObject>("Items/LastItem");

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
