using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Chaser : Enemy
{
    private DetectionSystem ds;
    private NavMeshAgent agent;
    public float speed;
    private List<GameObject> players;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ds = GetComponent<DetectionSystem>();

        players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
        players.Sort((j1,j2) => (int) (Vector3.Distance(j1.transform.position,transform.position) - Vector3.Distance(j2.transform.position,transform.position)));
        GameObject closerPlayer = players.First();
        ds.aDetectar = closerPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        players.Sort((j1,j2) => (int) (Vector3.Distance(j1.transform.position,transform.position) - Vector3.Distance(j2.transform.position,transform.position)));
        GameObject closerPlayer = players.First();
        ds.aDetectar = closerPlayer.transform;
        
        if (ds.localizado)
        {
            agent.speed = speed;
            agent.SetDestination(ds.aDetectar.position);
        }
    }

    void OnCollisionEnter(Collision other) {

        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<Bullet>().damage;

            if (health <= 0)
            {
                Destroy(this.gameObject);
            }

        }


    }

    void OnCollisionStay(Collision other) {
        
    }

}
