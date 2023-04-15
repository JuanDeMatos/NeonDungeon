using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Creeper : Enemy
{
    //public Transform trail;
    private NavMeshAgent agent;
    public GameObject creepCollider;
    public TrailRenderer trail;
    public float speed;
    public float moveRange;
    private float acum;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        StartCoroutine(Move());
    }

    // Update is called once per frame
    void Update()
    {
        acum = (acum + 1) % 40;

        if (acum == 0)
        {
            GameObject copy = Instantiate(creepCollider,trail.transform.position,Quaternion.Euler(transform.eulerAngles));
            Destroy(copy,trail.time);
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
            Debug.Log(randomDirection);
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
                Destroy(this.gameObject);
            }

        } else if (other.gameObject.tag == "Obstacle") {
            ChangeDirection();
        }
    }

    void OnCollisionStay(Collision other) {
        
    }

}
