using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpawner : MonoBehaviour
{
    public NetworkObject prefabReference;
    /*
    public void Awake()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
            SpawnNetworkObject();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */
    public void Spawn()
    {

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

}
