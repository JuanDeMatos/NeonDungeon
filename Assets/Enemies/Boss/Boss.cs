using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Unity.Netcode;

public class Boss : Enemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform[] exit;
    [SerializeField] private Transform[] bulletPivot;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;

    private List<GameObject> players;
    private GameObject closestPlayer;
    private float timeAcumulator;
    private float nextAttackTimer = 3f;

    [Range(0.1f,0.5f)]
    public float shootSpeed;
    public float bulletSpeed;
    public float chargeImpulse;
    public float chaseSpeed;

    public delegate void BossSpawnedHandler(Transform boss);
    public static event BossSpawnedHandler OnBossSpawned;
    public delegate void BossDeathHandler();
    public static event BossDeathHandler OnBossDeath;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
            this.Invoke(() => {
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            },1f);
        }
        OnBossSpawned(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (players == null || players.Count <= 0)
            return;

        players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
        players.Sort((j1, j2) => (int)(Vector3.Distance(j1.transform.position, transform.position) - Vector3.Distance(j2.transform.position, transform.position)));
        closestPlayer = players.First();
        
        timeAcumulator += Time.deltaTime;

        if (timeAcumulator >= nextAttackTimer)
        {
            timeAcumulator = 0;
            nextAttackTimer = Random.Range(5f, 15f);
            NextAttack();
        }

    }

    void NextAttack()
    {
        agent.isStopped = true;
        StopAllCoroutines();
        switch (Random.Range(0,3))
        {
            case 0:
                StartCoroutine(RotateBulletExits());
                StartCoroutine(Shoot());
                break;
            case 1:
                StartCoroutine(Charge());
                break;
            case 2:
                StartCoroutine(ChaseCloserPlayer());
                break;
;        }
    }

    IEnumerator RotateBulletExits()
    {
        while (true)
        {
            foreach (Transform t in bulletPivot)
            {
                t.Rotate(90 * Time.deltaTime * Vector3.up);
            }
            yield return new WaitForEndOfFrame();
        }
        
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            for (int i = 0; i < exit.Length; i++)
            {
                GameObject copy = Instantiate(bulletPrefab, exit[i].position, Quaternion.Euler(transform.eulerAngles));
                copy.GetComponent<Bullet>().gravity = 0;
                copy.GetComponent<Enemy>().damage = this.damage;

                copy.GetComponent<NetworkObject>().Spawn(true);
                copy.GetComponent<Rigidbody>().AddForce(exit[i].transform.forward * bulletSpeed, ForceMode.VelocityChange);

                Destroy(copy, 10f);
                yield return new WaitForEndOfFrame();
            }
            
            yield return new WaitForSeconds(shootSpeed);
        }
    }

    IEnumerator Charge()
    {
        while (true)
        {
            rb.AddForce( Vector3.Normalize(closestPlayer.transform.position - transform.position) * chargeImpulse, ForceMode.Impulse);

            yield return new WaitForSeconds(2f);
        }
    }



    IEnumerator ChaseCloserPlayer()
    {
        while (true)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(closestPlayer.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Bullet")
        {
            health -= other.gameObject.GetComponent<Bullet>().damage;

            if (health <= 0)
            {
                base.Death();
                this.GetComponent<NetworkObject>().Despawn(true);
                OnBossDeath();
            }

        }
    }

    void OnCollisionStay(Collision other)
    {

    }

}
