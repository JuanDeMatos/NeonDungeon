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
        StartCoroutine(SpawnCreep());
    }

    IEnumerator SpawnCreep()
    {
        while (true)
        {
            GameObject copy = Instantiate(creepCollider, trail.transform.position, Quaternion.Euler(transform.eulerAngles));
            copy.GetComponent<Enemy>().damage = this.damage;
            Destroy(copy, trail.time);
            yield return new WaitForSeconds(0.2f);
        } 
    }

    // Update is called once per frame
    void Update()
    { 
        if (!agent.hasPath)
        {
            ChangeDirection();
        }
    }
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
        Destroy(trail);
    }
    
    void OnCollisionStay(Collision other) {
        
    }

}
