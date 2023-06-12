using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using QFSW.QC;
using UnityEngine.SceneManagement;
using Unity.Collections;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using TMPro;

public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> username;
    public CinemachineVirtualCamera mainCamera;
    [SerializeField] PlayerAudio playerAudio;

    [Header("Physhics and Required Objects")]
    public const float GRAVITY = -9.81f;
    public bool usesGravity;
    public Transform salidaBala;
    public GameObject prefabBala;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [Header("Player identification")]
    [SerializeField] private TextMeshPro usernameLabel;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    public Color[] colors;
    [SerializeField] private SpriteRenderer minimapDot;
    [SerializeField] private GameObject minimapCamera;


    [Header("Player Attributes")]
    public float maxHealth;
    public float health;
    public float movementSpeed;
    public bool vulnerable;
    public float invulnerableTime;
    public bool dashing;
    public int dashCharges;
    public float dashChargesAcumulator;
    public float dashSpeed;
    public float dashRange;
    public List<Item> itemList;
    // Boolean to manage dash not stopping when the dash stops and OnMove() isn't triggering
    private bool moving;
    public Vector3 movement;

    [Header("Bullet Properties")]
    public bool shooting;
    private float shootAcumulator = 0;
    public float shootSpeed;
    public float damage;
    public float bulletSpeed;
    public float range;

    private bool endInvulnerabilityStarted;

    public delegate void PlayerDeathHandler();
    public event PlayerDeathHandler OnPlayerDeath;
    public delegate void HitHandler();
    public static event HitHandler OnHit;

    private HUD hud = null;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();   
        if (!IsOwner) { 
            gameObject.GetComponent<PlayerInput>().enabled = false;
            this.enabled = false;
            Destroy(minimapCamera);
        } else
        {
            Debug.Log(Shared.username);
            SetPlayerIdServerRpc(Shared.username);
            StartCoroutine(SearchCamera());
        }

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        NetworkManager.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        LooseState.OnShutdown += LooseState_OnShutdown;

    }

    private void LooseState_OnShutdown()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        NetworkManager.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        LooseState.OnShutdown -= LooseState_OnShutdown;
        Destroy(this.gameObject);
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        ApplyPlayerPropertiesClientRpc();
    }

    [ServerRpc]
    void SetPlayerIdServerRpc(string username)
    {
        this.username.Value = username;
    }


    [ClientRpc]
    void ApplyPlayerPropertiesClientRpc()
    {
        StartApplyPlayerProperties();
    }

    public void StartApplyPlayerProperties()
    {
        StopCoroutine(ApplyPlayerProperties());
        StartCoroutine(ApplyPlayerProperties());
    }

    IEnumerator ApplyPlayerProperties()
    {
        while (true)
        {
            if (IsOwner)
            {
                usernameLabel.gameObject.SetActive(false);
            }

            usernameLabel.SetText(username.Value.ToString());
            usernameLabel.color = colors[OwnerClientId];
            meshRenderer.material.color = colors[OwnerClientId];
            usernameLabel.transform.rotation = Quaternion.Euler(60, 0, 0);
            minimapDot.color = colors[OwnerClientId];
            yield return new WaitForEndOfFrame();
        }

    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (!IsLocalPlayer)
            return;

        usesGravity = false;
        movement = Vector3.zero;
        
        if (mainCamera != null)
            mainCamera.Follow = transform;
        else
            StartCoroutine(SearchCamera());

        MoveToZero();
        usesGravity = true;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsLocalPlayer)
            return; 
        
        usesGravity = false;
        movement = Vector3.zero;
        
        if (mainCamera != null)
            mainCamera.Follow = transform;
        else
            StartCoroutine(SearchCamera());

        MoveToZero();
        usesGravity = true;
    }

    IEnumerator SearchCamera()
    {
        GameObject camera = null;
        do
        {
            
            camera = GameObject.Find("CM vcam1");
            if (camera != null)
            {
                mainCamera = camera.GetComponent<CinemachineVirtualCamera>();
                mainCamera.m_Follow = this.transform;
            }
            yield return new WaitForSeconds(0.3f);

        } while (camera == null);

    }

    // Update is called once per frame
    void Update()
    {
        if (PauseScript.paused)
            return;

        if (_controller.isGrounded && _controller.velocity.y < 0)
        {
            movement.y = 0f;
        } else {
            if (usesGravity)
                movement.y = GRAVITY;
        }

        _controller.Move(movementSpeed * movement * Time.deltaTime);

        Shoot();

        if (!dashing && dashChargesAcumulator < dashCharges)
            dashChargesAcumulator += Time.deltaTime;
    }

    void Shoot()
    {
        if (shootAcumulator != 0 && !shooting)
        {
            shootAcumulator += Time.deltaTime;
            if (shootAcumulator >= shootSpeed)
                shootAcumulator = 0;
        }


        if (shooting)
        {
            if (shootAcumulator == 0)
                SpawnBulletServerRpc();

            shootAcumulator += Time.deltaTime;

            if (shootAcumulator >= shootSpeed / 10)
            {
                shootAcumulator = 0;
                SpawnBulletServerRpc();
            }
        }
    }

    void OnMove(InputValue value) {

        if (PauseScript.paused)
            return;
        
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

        if (PauseScript.paused)
            return;

        Vector2 v = value.Get<Vector2>();

        if (v != Vector2.zero)
        {
            /*
            float rotacion = Mathf.Rad2Deg * Mathf.Asin(v.y) - 90;
            rotacion = v.x>0?-rotacion:rotacion;

            transform.eulerAngles = new Vector3(0,rotacion,0);
            */

            transform.LookAt(transform.position + (new Vector3(v.x, 0, v.y) * 10));
            Debug.DrawLine(transform.position, transform.position + (new Vector3(v.x, 0, v.y) * 10));
            transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0,20,0));

        }

    }

    void OnAimMouse(InputValue value)
    {
        if (PauseScript.paused)
            return;

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

        if (PauseScript.paused)
            return;

        shooting = value.Get<float>()>0;

    }

    [ServerRpc]
    void SpawnBulletServerRpc ()
    {
        GameObject clon = Instantiate(prefabBala, salidaBala.position, Quaternion.Euler(salidaBala.eulerAngles));

        clon.GetComponent<Bullet>().gravity = GRAVITY;
        clon.GetComponent<Bullet>().damage = this.damage;
        float size = (damage + 9) / 100;
        size = size < 0.1f ? 0.1f : size;
        clon.transform.localScale = Vector3.one * size;

        clon.GetComponent<NetworkObject>().Spawn(true);
        clon.GetComponent<Rigidbody>().AddForce(clon.transform.forward * bulletSpeed, ForceMode.VelocityChange);
        clon.GetComponent<Rigidbody>().AddForce(Vector3.up * range, ForceMode.VelocityChange);

        Destroy(clon, 10f);
    }

    public void StartDash() {
        playerAudio.PlayDash();
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

        if (PauseScript.paused)
            return;

        if (dashing || dashChargesAcumulator < 1)
            return;

        dashChargesAcumulator--;
        StartDash();
        Invoke("StopDash", dashRange / dashSpeed);
    }

    void OnPause()
    {
        FindObjectOfType<PauseScript>().Pause();
    }

    void OnCollisionEnter(Collision other) {
        if (IsOwner)
            EvaluateCollision(other.gameObject);

    }

    void OnCollisionStay(Collision other) {
        if (IsOwner)
            EvaluateCollision(other.gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if (IsOwner)
            EvaluateCollision(other.gameObject);
    }

    void OnTriggerStay(Collider other) {
        if (IsOwner)
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

            if (!IsLocalPlayer)
                return;

            OnHit();

            if (health <= 0)
            {
                GetComponent<PlayerDeathScript>().dead = true;
                SetDeadServerRpc();
                StartSpectating();

                SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
                NetworkManager.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
                NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
                OnPlayerDeath();
            }
        }
    }

    [ServerRpc]
    void SetDeadServerRpc()
    {
        SetDeadClientRpc();
    }


    [ClientRpc]
    void SetDeadClientRpc()
    {
        GetComponent<PlayerDeathScript>().dead = true;
    }

    public void StartSpectating()
    {
        mainCamera.Follow = null;
        MoveToZero();
        GetComponent<PlayerDeathScript>().DeactivatePlayerServerRpc(OwnerClientId);
        FindObjectOfType<Spectator>().SpectateAlivePlayer();
    }

    public void MoveToZero()
    {
        GetComponent<ClientNetworkTransform>().Teleport(Vector3.zero, Quaternion.identity, Vector3.one);
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

    public void Heal(int amount)
    {
        if (health + amount > maxHealth)
            health = maxHealth;
        else
            health += amount;
    }
    void OnBigMinimap(InputValue value)
    {
        if (hud == null)
        {
            hud = FindObjectOfType<HUD>();
        }

        hud?.BigMinimap(value.Get<float>() == 1);
    }


}
