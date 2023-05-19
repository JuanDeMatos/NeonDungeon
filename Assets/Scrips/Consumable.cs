using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Consumable : NetworkBehaviour
{
    public ConsumableType type;
    public int amount;

    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag("Player"))
            return;

        AddConsumable();

        if (IsServer)
            Despawn();
        
    }
    
    void AddConsumable()
    {
        switch (type)
        {
            case (ConsumableType.Key):
                FindObjectOfType<SharedInventory>().AddKeys(amount);
                break;
            case (ConsumableType.Coin):
                FindObjectOfType<SharedInventory>().AddCoins(amount);
                break;
            default:
                return;
        }
    }

    void Despawn()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    
}

public enum ConsumableType
{
    Key,Coin
}
