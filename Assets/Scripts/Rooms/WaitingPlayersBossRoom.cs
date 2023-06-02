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

    private Seed seed;

    private void Start()
    {
        if (Shared.gameMode != GameMode.Coop)
            this.gameObject.SetActive(false);
    }

    void Update()
    {
        text.transform.eulerAngles = new Vector3(60, 0, 0);
        if (seed == null)
            seed = FindObjectOfType<Seed>();
        else
        {
            text.SetText(numberPlayersReady.Value + " / " + seed.CountPlayers());
            if (numberPlayersReady.Value == seed.CountPlayers())
                OnAllPlayersReady();
        }

        
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
