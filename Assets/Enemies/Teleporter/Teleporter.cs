using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Unity.Netcode;

public class Teleporter : Enemy
{
    [SerializeField]
    private NavMeshAgent agent;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float range;
    public float teleportRange;
    public Transform exit;
    private List<GameObject> players;
    private const float GRAVITY = -9.81f;
    public bool detected;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
            detected = false;
            StartCoroutine(Teleport());
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

            Vector3 direction = closerPlayer.transform.position - transform.position;

            if (Physics.Raycast(transform.position, direction, 1000, LayerMask.GetMask("Player")))
            {
                detected = true;
                transform.LookAt(closerPlayer.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }

    void Shoot() {
        GameObject copy = Instantiate(bulletPrefab,exit.position,Quaternion.Euler(transform.eulerAngles));
                copy.GetComponent<Rigidbody>().AddForce(copy.transform.forward * bulletSpeed,ForceMode.VelocityChange);
                copy.GetComponent<Bullet>().gravity = GRAVITY + range;
                copy.GetComponent<Enemy>().damage = this.damage;
                
                Destroy(copy,10f);
    }

    IEnumerator Teleport() {
        while (true) {
            yield return new WaitForSeconds(2);
            Vector3 position;
            do
            {
                Vector3 randomDirection = Random.insideUnitSphere * teleportRange;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection,out hit,teleportRange,1);
                position = hit.position;
                

            } while (!agent.SetDestination(position));

            agent.Warp(position);

            if (detected)
            {
                yield return new WaitForSeconds(0.3f);
                GameObject copy = Instantiate(bulletPrefab,exit.position,Quaternion.Euler(transform.eulerAngles));
                copy.GetComponent<Bullet>().gravity = GRAVITY + range;
                copy.GetComponent<Enemy>().damage = this.damage;

                copy.GetComponent<NetworkObject>().Spawn(true);
                copy.GetComponent<Rigidbody>().AddForce(copy.transform.forward * bulletSpeed,ForceMode.VelocityChange);
                
                Destroy(copy,10f);
            }
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
