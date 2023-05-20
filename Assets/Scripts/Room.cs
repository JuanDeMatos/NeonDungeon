using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<GameObject> positions;

    // Start is called before the first frame update
    void Awake()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Invoke("SpawnEnemies", 10);
        } else
        {

           positions.ForEach(p => Destroy(p));

        }
        
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            
            GameObject e = enemies[i];
            Transform p = positions[i].transform;
            GameObject copy = Instantiate(e, p.position, p.rotation);
            Destroy(p.gameObject);
            copy.GetComponent<NetworkObject>().Spawn();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
