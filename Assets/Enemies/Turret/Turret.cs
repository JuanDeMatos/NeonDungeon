using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Unity.Netcode;

public class Turret : Enemy
{
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public float range;
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
            InvokeRepeating("Shoot", 0, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (players == null || players.Count <= 0)
            return;

        players = GameObject.FindGameObjectsWithTag("PlayerDetection").ToList();
        players.Sort((j1, j2) => (int)(Vector3.Distance(j1.transform.position, transform.position) - Vector3.Distance(j2.transform.position, transform.position)));
        GameObject closerPlayer = players.First();

        Vector3 direction = closerPlayer.transform.position - transform.position;

        if (!detected)
        {
            Debug.DrawRay(transform.position, direction * 1000);
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit, 1000);

            if (hit.collider.CompareTag("Player"))
            {
                detected = true;
                transform.LookAt(closerPlayer.transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
        else
        {
            transform.LookAt(closerPlayer.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

    }

    void Shoot()
    {
        if (!detected)
            return;

        GameObject copy = Instantiate(bulletPrefab, exit.position, Quaternion.Euler(transform.eulerAngles));
        copy.GetComponent<Bullet>().gravity = GRAVITY + range;
        copy.GetComponent<Enemy>().damage = this.damage;

        copy.GetComponent<NetworkObject>().Spawn(true);
        copy.GetComponent<Rigidbody>().AddForce(copy.transform.forward * bulletSpeed, ForceMode.VelocityChange);

        Destroy(copy, 10f);

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
            }

        }
    }

    void OnCollisionStay(Collision other)
    {

    }

}
