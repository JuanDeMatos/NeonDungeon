using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    // Start is called before the first frame update

    public float gravity;
    public float damage;

    void Awake()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * gravity, ForceMode.Acceleration);

        //Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    void OnCollisionEnter(Collision other) {

        this.GetComponent<NetworkObject>().Despawn(true);
        //Destroy(this.gameObject);
        
    }
}
