using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ItemRoom : MonoBehaviour
{
    [SerializeField] private GameObject roof;
    [SerializeField] private GameObject doors;
    [SerializeField] private GameObject doorTriggers;
    private Seed seed;

    private void Start()
    {
        seed = FindObjectOfType<Seed>();
        //Random.InitState(seed.GetSeed());
    }

    public void StartRoom()
    {
        roof.SetActive(false);
        doors.SetActive(false);
        doorTriggers.SetActive(false);
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        
        List<ObjectSpawner> spawners = GetComponentsInChildren<ObjectSpawner>().ToList();

        for (int i = 0; i < spawners.Count; i++)
        {

            if (seed.availableItems.Count > 0)
            {
                int random = Shared.criticalRandomGenerator.Next(0, seed.availableItems.Count);
                Debug.Log("Random Item: " + random);
                spawners[i].prefabReference = seed.availableItems[random].GetComponent<NetworkObject>();
                seed.availableItems.RemoveAt(random);
            } else
            {
                spawners[i].prefabReference = seed.lastItem.GetComponent<NetworkObject>();
            }
                
            spawners[i].Spawn();
            
            yield return new WaitForSeconds(0.1f);
        }
    }
}

