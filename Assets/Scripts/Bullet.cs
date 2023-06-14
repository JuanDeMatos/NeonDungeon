using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    // Start is called before the first frame update

    public float gravity;
    public float damage;

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision other) {

        if (other.gameObject.CompareTag("Player"))
            if (!other.gameObject.GetComponent<Player>().vulnerable)
                return;

        if (this.GetComponent<NetworkObject>().IsSpawned)
            this.GetComponent<NetworkObject>().Despawn(true);
        
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
            if (!other.GetComponent<Player>().vulnerable)
                return;

        if (this.GetComponent<NetworkObject>().IsSpawned)
            this.GetComponent<NetworkObject>().Despawn(true);

    }
}
