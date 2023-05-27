using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class ShopItem : NetworkBehaviour
{
    public int price;
    [SerializeField] TextMeshPro priceText;

    public override void OnNetworkSpawn()
    {
        if (Random.Range(0, 10f) <= 5)
            price /= 2;

        priceText.SetText(price + "");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        FindObjectOfType<SharedInventory>().UseCoins(price);

        other.GetComponent<Player>().AddItem(GetComponent<Item>());

        if (IsServer)
            GetComponent<NetworkObject>().Despawn();
    }

}
