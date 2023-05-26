using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using QFSW.QC;

public class Player : NetworkBehaviour
{
    [Header("Physhics and Required Objects")]
    public const float GRAVITY = -9.81f;
    public bool usesGravity;
    public Transform salidaBala;
    public GameObject prefabBala;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;

    // Boolean to manage dash not stopping when the dash stops and OnMove() isn't triggering
    private bool moving;
    public Vector3 movement;

    [Header("Player Attributes")]
    public float health;
    public float movementSpeed;
    public bool vulnerable;
    public float invulnerableTime;
    public bool dashing;
    public float dashSpeed;
    public float dashDuration;
    public List<Item> itemList;

    [Header("Bullet Properties")]
    public float damage;
    public float bulletSpeed;
    public float range;

    private bool endInvulnerabilityStarted;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) { 
            gameObject.GetComponent<PlayerInput>().enabled = false;
            this.enabled = false;
        } else
        {
            StartCoroutine(SearchCamera());
        }

    }

    IEnumerator SearchCamera()
    {
        GameObject camera = null;
        do
        {
            
            camera = GameObject.Find("CM vcam1");
            if (camera != null)
            {
                camera.GetComponent<CinemachineVirtualCamera>().m_Follow = this.transform;
            }
            yield return new WaitForSeconds(0.3f);

        } while (camera == null);

    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.isGrounded && _controller.velocity.y < 0)
        {
            movement.y = 0f;
        } else {
            if (usesGravity)
                movement.y = GRAVITY;
        }

        _controller.Move(movementSpeed * movement * Time.deltaTime);

    }

    void OnMove(InputValue value) {
        
        Vector2 v = value.Get<Vector2>();

        if (!dashing)
        {
            movement = Vector3.right * v.x + new Vector3(0,_controller.velocity.y,0) + Vector3.forward * v.y;

            if (v == Vector2.zero)
            {
                moving = false;
                movement = new Vector3(0,movement.y,0);
            } else
            {
                moving = true;
            }
        }

        if (v == Vector2.zero)
        {
            moving = false;
        }
    }
    
    void OnAim(InputValue value) {

        Vector2 v = value.Get<Vector2>();

        if (v != Vector2.zero)
        {
            float rotacion = Mathf.Rad2Deg * Mathf.Asin(v.y) - 90;
            rotacion = v.x>0?-rotacion:rotacion;

            transform.eulerAngles = new Vector3(0,rotacion,0);
        }

    }

    void OnAimMouse(InputValue value)
    {
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Floor"));

            transform.LookAt(hit.point);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 20, 0);
        }

    }

    void OnFire(InputValue value) {

        SpawnBulletServerRpc();

    }

    [ServerRpc]
    void SpawnBulletServerRpc ()
    {
        GameObject clon = Instantiate(prefabBala, salidaBala.position, Quaternion.Euler(salidaBala.eulerAngles));

        clon.GetComponent<Bullet>().gravity = GRAVITY + range;
        clon.GetComponent<Bullet>().damage = this.damage;
        float size = damage / 8;
        size = size < 1 ? size : 1;
        clon.transform.localScale = new Vector3(size, size, size);

        clon.GetComponent<NetworkObject>().Spawn(true);
        clon.GetComponent<Rigidbody>().AddForce(clon.transform.forward * bulletSpeed, ForceMode.VelocityChange);
        clon.GetComponent<Rigidbody>().AddForce(GetComponent<CharacterController>().velocity / 3f, ForceMode.Impulse);

        Destroy(clon, 10f);
    }

    public void StartDash() {
        dashing = true;
        vulnerable = false;
        movementSpeed *= dashSpeed;
    }

    public void StopDash() {
        dashing = false;

        if (!moving)
            movement = Vector3.zero;

        movementSpeed /= dashSpeed;

        if (!endInvulnerabilityStarted)
            EndInvulnerability();
    }

    void OnDash(InputValue value) {
        StartDash();
        Invoke("StopDash",dashDuration);
    }

    void OnCollisionEnter(Collision other) {
        EvaluateCollision(other.gameObject);

    }

    void OnCollisionStay(Collision other) {
        EvaluateCollision(other.gameObject);
    }

    void OnTriggerEnter(Collider other) {
        EvaluateCollision(other.gameObject);
    }

    void OnTriggerStay(Collider other) {
        EvaluateCollision(other.gameObject);
    }

    void EvaluateCollision(GameObject other)
    {
        if ( (other.CompareTag("Enemy") || other.CompareTag("EnemyProjectile")) && vulnerable)
        {
            vulnerable = false;
            endInvulnerabilityStarted = true;
            Invoke("EndInvulnerability", invulnerableTime);

            Enemy enemy = other.GetComponent<Enemy>();

            health -= enemy.damage;
            Debug.Log(health);

            if (health <= 0)
            {
                this.GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
        item.ApplyItem(this);
    }

    public void RemoveRandomItem()
    {
        if (itemList.Count > 0)
        {
            int randomPosition = Random.Range(0, itemList.Count);
            Item item = itemList[randomPosition];
            item.ClearItem(this);
            itemList.RemoveAt(randomPosition);
        }
        
    }

    void EndInvulnerability() {
        vulnerable = true;
        endInvulnerabilityStarted = false;
    }

    

}
