using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{

    [Header("Enemy Attributes")]

    public float health;
    public float damage;

    private void Awake()
    {
        this.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            this.enabled = true;
            int nPlayers = FindObjectOfType<Seed>().CountPlayers();
            health *= nPlayers;
            damage *= nPlayers;
        }
    }


}
