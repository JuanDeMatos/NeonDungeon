using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using System;
using TMPro;
using UnityEngine.EventSystems;

public class Spectator : MonoBehaviour
{
    public int lookingAtPlayer;
    public List<GameObject> alivePlayers;
    public CinemachineVirtualCamera mainCamera;
    public GameObject spectatorCanvas;
    public GameObject HUDCanvas;
    public TextMeshProUGUI playerSpectating;
    [SerializeField] GameObject nextButton;

    // Start is called before the first frame update
    void Start()
    {
        alivePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        enabled = false;
        this.Invoke(() => spectatorCanvas.SetActive(false), 0.001f);
    }

    // Update is called once per frame
    void Update()
    {
        alivePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
    }

    public void Disable()
    {
        HUDCanvas.SetActive(true);
        spectatorCanvas.SetActive(false);
        enabled = false;
    }

    public void SpectateAlivePlayer()
    {
        this.enabled = true;
        HUDCanvas.SetActive(false);
        spectatorCanvas.SetActive(true);
        FindObjectOfType<EventSystem>().SetSelectedGameObject(nextButton);
        this.Invoke(() => {
            GameObject objective = alivePlayers.Find(go => go.activeSelf);
            if (objective != null)
            {
                mainCamera.Follow = objective.transform;
                playerSpectating.SetText("Player: " + objective.GetComponent<Player>().OwnerClientId +
                    " " + objective.GetComponent<Player>().username.Value);
            }   
        }, 1f);
        
    }

    public void NextPlayer()
    {
        Debug.Log("Next");
        lookingAtPlayer++;
        if (lookingAtPlayer >= alivePlayers.Count)
            lookingAtPlayer = 0;

        Transform playerFollowing = alivePlayers[lookingAtPlayer].transform;

        mainCamera.Follow = playerFollowing;
        playerSpectating.SetText("Player: " + playerFollowing.GetComponent<Player>().OwnerClientId + 
            " " + playerFollowing.GetComponent<Player>().username.Value);
    }

    public void PreviousPlayer()
    {
        Debug.Log("Previous");
        lookingAtPlayer--;
        if (lookingAtPlayer < 0)
            lookingAtPlayer = alivePlayers.Count - 1;

        Transform playerFollowing = alivePlayers[lookingAtPlayer].transform;

        mainCamera.Follow = playerFollowing;
        playerSpectating.SetText("Player: " + playerFollowing.GetComponent<Player>().OwnerClientId + 
            " " + playerFollowing.GetComponent<Player>().username.Value);
    }
}
