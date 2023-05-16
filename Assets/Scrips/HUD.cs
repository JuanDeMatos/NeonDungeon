using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textKeys;
    [SerializeField] TextMeshProUGUI textCoins;

    // Start is called before the first frame update
    void Start()
    {
        SharedInventory sharedInventory = FindObjectOfType<SharedInventory>();
        textKeys.SetText("Keys: " + sharedInventory.keys.Value);
        textCoins.SetText("Coins: " + sharedInventory.coins.Value);

        sharedInventory.OnKeysModified += (int n) =>
        {
            SharedInventory sharedInventory = FindObjectOfType<SharedInventory>();
            Debug.Log("Event " + n);
            textKeys.SetText("Keys: " + n);
        };
        sharedInventory.OnCoinsModified += (int n) =>
        {
            SharedInventory sharedInventory = FindObjectOfType<SharedInventory>();
            Debug.Log("Event " + n);
            textCoins.SetText("Coins: " + n);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
