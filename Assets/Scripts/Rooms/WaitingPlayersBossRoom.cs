using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
public class WaitingPlayersBossRoom : MonoBehaviour
{
    [SerializeField] TextMeshPro text;
    public NetworkVariable<int> numberPlayersReady;

    public delegate void AllPlayersReady();
    public event AllPlayersReady OnAllPlayersReady;

    void Update()
    {
        if (numberPlayersReady.Value == FindObjectOfType<Seed>().CountPlayers())
            OnAllPlayersReady();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        AddPlayerServerRpc();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        RemovePlayerServerRpc();

    }

    [ServerRpc]
    void AddPlayerServerRpc()
    {
        numberPlayersReady.Value++;
    }

    [ServerRpc]
    void RemovePlayerServerRpc()
    {
        numberPlayersReady.Value--;
    }
}
