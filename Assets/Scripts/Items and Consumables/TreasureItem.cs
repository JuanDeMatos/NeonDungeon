using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TreasureItem : NetworkBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            GameObject playerGameObject = other.gameObject;
            Player player = playerGameObject.GetComponent<Player>();
            player.AddItem(GetComponent<Item>());

            if (IsServer)
                GetComponent<NetworkObject>().Despawn();
        }
    }
}
