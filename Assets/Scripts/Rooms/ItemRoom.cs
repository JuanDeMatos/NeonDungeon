using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ItemRoom : Room
{
    
    public override void StartRoom()
    {
        roof.SetActive(false);
        doors.SetActive(false);
        doorTriggers.SetActive(false);
        StartCoroutine(SpawnObjects());
    }

    protected override IEnumerator SpawnObjects()
    {
        
        List<ObjectSpawner> spawners = GetComponentsInChildren<ObjectSpawner>().ToList();

        for (int i = 0; i < spawners.Count; i++)
        {

            spawners[i].Spawn();
                
            yield return new WaitForSeconds(0.1f);
        }
    }
}

