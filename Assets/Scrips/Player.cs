using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    [Header("Player Attributes")]
    public float health;
    public float damage;
    public float movementSpeed;

    [Header("Bullet Properties")]
    public float bulletSpeed;
    public float range;
    public bool dashing;
    // Boolean to manage dash not stopping when the dash stops and OnMove() isn't triggering
    public bool moving;
    public bool vulnerable;
    public Vector3 movement;
    [SerializeField] private CharacterController _controller;

    [SerializeField] private Animator _animator;
    public float dashSpeed;
    public float dashDuration;
    public Transform salidaBala;
    public GameObject prefabBala;
    public const float GRAVITY = -9.81f;
    public bool usesGravity;


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
            yield return new WaitForEndOfFrame();
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

            Physics.Raycast(ray, out hit, 100);

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

        Destroy(clon, 10f);
    }

    public void StartDash() {
        dashing = true;
        movementSpeed *= dashSpeed;
    }

    public void StopDash() {
        dashing = false;

        if (!moving)
            movement = Vector3.zero;

        movementSpeed /= dashSpeed;
    }

    void OnDash(InputValue value) {
        StartDash();

        //_animator.SetTrigger("Rueda");

        Invoke("StopDash",dashDuration);

    }

    void OnCollisionEnter(Collision other) {
        Debug.Log("Collision Enter");
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;
            
            if (health <= 0)
            {
                this.GetComponent<NetworkObject>().Despawn(true);
            }

        }
    }

    void OnCollisionStay(Collision other) {
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;

            if (health <= 0)
            {
                this.GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;

            if (health <= 0)
            {
                this.GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;

            if (health <= 0)
            {
                this.GetComponent<NetworkObject>().Despawn(true);
            }
        }
    }

    void EndInvulnerability() {
        vulnerable = true;
    }

}
