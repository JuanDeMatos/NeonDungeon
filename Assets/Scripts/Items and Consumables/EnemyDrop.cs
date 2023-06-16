using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyDrop : NetworkBehaviour
{

    [SerializeField] Consumable coin,key,heart;
    public float startingChance;
    private float chance;

    private void Start()
    {
        chance = startingChance + (FindObjectOfType<Seed>().CountPlayers() * 5);
        Enemy.OnDrop += Enemy_OnDrop;
    }

    private void Enemy_OnDrop(Vector3 position)
    {
        if (!IsServer)
            return;

        chance += 0.5f;

        if (Random.Range(0f,100f) <= chance)
        {
            chance = startingChance;

            position.y = 0;

            float random = Random.Range(0, 100f);

            GameObject copy,randomConsumable;

            if (random >= 90)
                randomConsumable = heart.gameObject;
            else if (random >= 70 - (FindObjectOfType<Seed>().CountPlayers() * 2.5f))
                randomConsumable = key.gameObject;
            else
                randomConsumable = coin.gameObject;

            copy = Instantiate(randomConsumable, position, Quaternion.identity);
            copy.GetComponent<NetworkObject>().Spawn();

        }

    }
}
