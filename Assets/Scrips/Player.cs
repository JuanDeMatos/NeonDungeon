using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Attributes")]
    public float health;
    public float damage;
    public float movementSpeed;

    [Header("Bullet Properties")]
    public float bulletSpeed;
    public float range;
    public bool dashing;
    public bool vulnerable;
    public Vector3 movement;
    private CharacterController _controller;

    private Animator _animator;
    public float dashSpeed;
    public float dashDuration;
    public Transform salidaBala;
    public GameObject prefabBala;
    public const float GRAVITY = -9.81f;
    
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_controller.isGrounded && _controller.velocity.y < 0)
        {
            movement.y = 0f;
        } else {
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
            movement = Vector3.zero;
            }
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

    void OnAimMouse(InputValue value) {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, 100);

        transform.LookAt(hit.point);
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y + 20,0);

    }

    void OnFire(InputValue value) {

        GameObject clon = Instantiate(prefabBala,salidaBala.position,Quaternion.Euler(salidaBala.eulerAngles));

        clon.GetComponent<Rigidbody>().AddForce(clon.transform.forward * bulletSpeed,ForceMode.VelocityChange);
        clon.GetComponent<Bullet>().gravity = GRAVITY + range;
        clon.GetComponent<Bullet>().damage = this.damage;
        float size = damage / 8;
        size = size<1?size:1;
        clon.transform.localScale = new Vector3(size,size,size);
        
        Destroy(clon,10f);

    }

    public void StartDash() {
        dashing = true;
        movementSpeed *= dashSpeed;
    }

    public void StopDash() {
        dashing = false;
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
        }
    }

    void OnCollisionStay(Collision other) {
        Debug.Log("Collision Stay");
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger Enter");
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;
        }
    }

    void OnTriggerStay(Collider other) {
        Debug.Log("Trigger Stay");
        if (other.gameObject.tag == "Enemy" && vulnerable)
        {
            vulnerable = false;
            Invoke("EndInvulnerability",0.5f);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            health -= enemy.damage;
        }
    }

    void EndInvulnerability() {
        vulnerable = true;
    }

}