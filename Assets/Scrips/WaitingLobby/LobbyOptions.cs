using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyOptions : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI joinCodeText;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Scene loaded");
        joinCodeText.text += Shared.joinCode;
        Invoke("MovePlayer", 2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }
    }

    void MovePlayer()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player").ToList().Find(go => go.GetComponent<Player>().IsOwner);
        
        player.transform.position = Vector3.zero;
        player.GetComponent<Player>().usesGravity = true;
    }
    
}
