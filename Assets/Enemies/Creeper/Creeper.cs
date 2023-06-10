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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            GameObject trailCopy = Instantiate(trail.gameObject, transform);
            trailCopy.GetComponent<NetworkObject>().Spawn();
            trailCopy.transform.parent = transform;
            StartCoroutine(Move());
        }
        StartCoroutine(SpawnCreep());
    }

    void Update()
    {
        if (!agent.hasPath)
        {
            ChangeDirection();
        }
    }

    IEnumerator SpawnCreep()
    {
        while (true)
        {
            GameObject copy = Instantiate(creepCollider, new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 
                Quaternion.Euler(transform.eulerAngles));
            copy.GetComponent<Enemy>().damage = this.damage;
            Destroy(copy, trail.time);
            yield return new WaitForSeconds(0.15f);
        }
    }

    // Update is called once per frame

    void ChangeDirection() {

        NavMeshPath path = new NavMeshPath();
        do
        {
            Vector3 randomDirection = (Random.insideUnitSphere * moveRange);
            Vector3 randomPosition = transform.position + randomDirection;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPosition,out hit,10,1))
                agent.CalculatePath(hit.position, path);

        } while (path.status != NavMeshPathStatus.PathComplete);

        agent.SetPath(path);
    }

    IEnumerator Move() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            ChangeDirection();
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
                Invoke("DestroyTrail",trail.time);
                
            }

        } else if (other.gameObject.tag == "Obstacle") {
            ChangeDirection();
        }
    }
    
    void DestroyTrail()
    {
        trail.GetComponent<NetworkObject>().Despawn();
    }
    
    void OnCollisionStay(Collision other) {
        
    }

}
