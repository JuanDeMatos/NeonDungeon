using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class ShopItem : NetworkBehaviour
{
    public int price;
    public float halfPricePercentage;
    [SerializeField] TextMeshPro priceText;

    public override void OnNetworkSpawn()
    {
        if (Random.Range(0, 100f) <= halfPricePercentage)
            price /= 2;

        priceText.SetText(price + "");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!FindObjectOfType<SharedInventory>().UseCoins(price))
            return;

        other.GetComponent<Player>().AddItem(GetComponent<Item>());

        if (IsServer)
            GetComponent<NetworkObject>().Despawn();
    }

}
