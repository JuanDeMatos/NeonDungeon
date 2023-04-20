using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Unity.Netcode;

public class Creeper : Enemy
{
    //public Transform trail;
    [SerializeField]
    private NavMeshAgent agent;
    public GameObject creepCollider;
    public TrailRenderer trail;
    public float speed;
    public float moveRange;
    private float acum;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            StartCoroutine(Move());
        }
    }

    // Update is called once per frame
    void Update()
    {
        acum = (acum + 1) % 40;

        if (acum == 0)
        {
            GameObject copy = Instantiate(creepCollider,trail.transform.position,Quaternion.Euler(transform.eulerAngles));
            Destroy(copy, trail.time);
        }

        if (!agent.hasPath)
        {
            ChangeDirection();
        }
    }
    void ChangeDirection() {
        Vector3 position;
        do
        {
            Vector3 randomDirection = (Random.insideUnitSphere * moveRange);
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection,out hit,moveRange,1);
            position = hit.position;
        } while (!agent.SetDestination(position));
    }

    IEnumerator Move() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(2f,4f));
            ChangeDirection();
        }
    }

    void OnCollisionEnter(Collision other) {

        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<Bullet>().damage;

            if (health <= 0)
            {
                trail.transform.parent = null;
                Invoke("DestroyTrail",trail.time);
                GetComponent<NetworkObject>().Despawn(true);
            }

        } else if (other.gameObject.tag == "Obstacle") {
            ChangeDirection();
        }
    }

    void DestroyTrail()
    {
        trail.GetComponent<NetworkObject>().Despawn(true);
    }

    void OnCollisionStay(Collision other) {
        
    }

}
