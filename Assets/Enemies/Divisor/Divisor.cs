using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Unity.Netcode;

public class Divisor : Enemy
{
    [SerializeField]
    private DetectionSystem ds;
    [SerializeField]
    private NavMeshAgent agent;
    public float speed;
    private List<GameObject> players;
    public GameObject sonEnemy;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
            players.Sort((j1, j2) => (int)(Vector3.Distance(j1.transform.position, transform.position) - Vector3.Distance(j2.transform.position, transform.position)));
            GameObject closerPlayer = players.First();
            ds.aDetectar = closerPlayer.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (players != null && players.Count > 0)
        {
            players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
            players.Sort((j1, j2) => (int)(Vector3.Distance(j1.transform.position, transform.position) - Vector3.Distance(j2.transform.position, transform.position)));
            GameObject closerPlayer = players.First();
            ds.aDetectar = closerPlayer.transform;

            if (ds.localizado)
            {
                agent.speed = speed;
                agent.SetDestination(ds.aDetectar.position);
            }
        }
    }

    void OnCollisionEnter(Collision other) {

        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<Bullet>().damage;

            if (health <= 0)
            {
                base.Death();
                GetComponent<NetworkObject>().Despawn(true);

                if (sonEnemy != null)
                {
                    GameObject clon;
                    clon = Instantiate(sonEnemy,transform.position + new Vector3(0.5f,0,0.5f),Quaternion.Euler(0,0,0));
                    clon.GetComponent<NetworkObject>().Spawn(true);

                    clon = Instantiate(sonEnemy,transform.position + new Vector3(-0.5f,0,0.5f),Quaternion.Euler(0,0,0));
                    clon.GetComponent<NetworkObject>().Spawn(true);

                    clon = Instantiate(sonEnemy,transform.position + new Vector3(0.5f,0,-0.5f),Quaternion.Euler(0,0,0));
                    clon.GetComponent<NetworkObject>().Spawn(true);

                    clon = Instantiate(sonEnemy,transform.position + new Vector3(-0.5f,0,-0.5f),Quaternion.Euler(0,0,0));
                    clon.GetComponent<NetworkObject>().Spawn(true);
                }

            }
        }
    }

    void OnCollisionStay(Collision other) {
        
    }

}
