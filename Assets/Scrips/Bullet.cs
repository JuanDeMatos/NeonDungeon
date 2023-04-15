using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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

        Destroy(this.gameObject);
        
    }
}
