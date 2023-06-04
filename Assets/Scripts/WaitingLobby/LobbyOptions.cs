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
    [SerializeField] WaitForPlayers waitForPlayers;

    // Start is called before the first frame update
    void Start()
    {
        if (Shared.gameMode == GameMode.Coop)
        {
            joinCodeText.text += Shared.joinCode;
            Invoke("MovePlayer", 0.5f);
        }
        else
        {
            joinCodeText.transform.parent.gameObject.SetActive(false);
        }

        waitForPlayers.OnAllPlayersReady += LoadLevel1;
    }

    private void LoadLevel1()
    {
        if (NetworkManager.Singleton.IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    void MovePlayer()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player").ToList().Find(go => go.GetComponent<Player>().IsOwner);
        
        player.transform.position = Vector3.zero;
        player.GetComponent<Player>().usesGravity = true;
    }
    
}
