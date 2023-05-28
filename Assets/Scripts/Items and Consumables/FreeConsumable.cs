using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FreeConsumable : NetworkBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player"))
            return;

        GetComponent<Consumable>().GetConsumable(other.GetComponent<Player>());

    }
}
