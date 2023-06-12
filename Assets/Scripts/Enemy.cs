using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{

    [Header("Score")]
    public int points;

    [Header("Enemy Attributes")]
    public float health;
    public float damage;

    public delegate void DeathHandler(float points);
    public static event DeathHandler OnDeath;
    public delegate void DropHandler(Vector3 position);
    public static event DropHandler OnDrop;

    private void Awake()
    {
        this.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            this.enabled = true;
            float nPlayers = FindObjectOfType<Seed>().CountPlayers();
            health *= nPlayers;
            damage += (FindObjectOfType<FloorGenerator>().floorLevel);
            damage *= (nPlayers / 2) < 1 ? 1 : (nPlayers / 2);

        }
        else
        {
            this.enabled = false;
        }
    }

    protected void Death()
    {
        OnDrop(transform.position);
        OnDeath(points);
    }

}
