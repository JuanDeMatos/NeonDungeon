using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textKeys;
    [SerializeField] TextMeshProUGUI textCoins;
    private SharedInventory sharedInventory;

    // Start is called before the first frame update
    void Start()
    {
        sharedInventory = FindObjectOfType<SharedInventory>();
        textKeys.SetText("Keys: " + sharedInventory.keys.Value);
        textCoins.SetText("Coins: " + sharedInventory.coins.Value);

        //sharedInventory.OnKeysModified += SharedInventory_OnKeysModified;

        //sharedInventory.OnCoinsModified += SharedInventory_OnCoinsModified;

        StartCoroutine(UpdateConsumables());
    }
    /*
    private void SharedInventory_OnCoinsModified(int n)
    {
        Debug.Log("Coins " + n);
        textCoins.SetText("Coins: " + n);
    }

    private void SharedInventory_OnKeysModified(int n)
    {
        Debug.Log("Keys " + n);
        textKeys.SetText("Keys: " + n);
    }
    */
    IEnumerator UpdateConsumables()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            textKeys.SetText("Keys: " + sharedInventory.keys.Value);
            textCoins.SetText("Coins: " + sharedInventory.coins.Value);
        } 
    }
}
