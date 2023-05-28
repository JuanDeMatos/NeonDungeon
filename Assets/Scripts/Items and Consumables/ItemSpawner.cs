using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : ObjectSpawner
{
    private Seed seed;
    public ItemType type;

    public override void Spawn()
    {
        seed = FindObjectOfType<Seed>();
        prefabReference = null;

        switch (type)
        {
            case ItemType.Treasure:
                GetTreasureItem();
                GetPermanentTreasureItem();
                break;
            case ItemType.Shop:
                GetShopItem();
                GetPermanentShopItem();
                break;
        }

        NetworkManager.Singleton.RemoveNetworkPrefab(prefabReference.gameObject);
        NetworkManager.Singleton.AddNetworkPrefab(prefabReference.gameObject);

        Debug.Log(prefabReference.gameObject.name);

        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
            NetworkObject instantiatedNetworkObject = Instantiate(prefabReference, transform.position, transform.rotation, null);

            instantiatedNetworkObject.Spawn(destroyWithScene: true);

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void GetTreasureItem() {

        if (prefabReference != null || seed.treasureItems.Count <= 0)
            return;

        int random = Shared.criticalRandomGenerator.Next(0, seed.treasureItems.Count);
        prefabReference = seed.treasureItems[random].GetComponent<NetworkObject>();
        seed.treasureItems.RemoveAt(random);
    }

    private void GetShopItem()
    {
        if (prefabReference != null || seed.shopItems.Count <= 0)
            return;

        int random = Shared.criticalRandomGenerator.Next(0, seed.shopItems.Count);
        prefabReference = seed.shopItems[random].GetComponent<NetworkObject>();
        seed.shopItems.RemoveAt(random);
    }

    private void GetPermanentTreasureItem()
    {
        if (prefabReference != null || seed.permanentTreasureItems.Count <= 0)
            return;

        int random = Shared.criticalRandomGenerator.Next(0, seed.permanentTreasureItems.Count);
        prefabReference = seed.permanentTreasureItems[random].GetComponent<NetworkObject>();
    }
    private void GetPermanentShopItem()
    {
        if (prefabReference != null || seed.permanentTreasureItems.Count <= 0)
            return;

        int random = Shared.criticalRandomGenerator.Next(0, seed.permanentShopItems.Count);
        prefabReference = seed.permanentShopItems[random].GetComponent<NetworkObject>();
    }



    public enum ItemType
    {
       Treasure,Shop
    }
}
