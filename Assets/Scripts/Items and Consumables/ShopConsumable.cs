using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class ShopConsumable : NetworkBehaviour
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

        GetComponent<Consumable>().GetConsumable(other.GetComponent<Player>());

    }
}
