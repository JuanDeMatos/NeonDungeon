using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using System;

public class Spectator : MonoBehaviour
{
    public int lookingAtPlayer;
    public List<GameObject> alivePlayers;
    public CinemachineVirtualCamera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        alivePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();
        mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        alivePlayers = GameObject.FindGameObjectsWithTag("Player").ToList();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Next");
            lookingAtPlayer++;
            if (lookingAtPlayer >= alivePlayers.Count)
                lookingAtPlayer = 0;

            mainCamera.Follow = alivePlayers[lookingAtPlayer].transform;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Previous");
            lookingAtPlayer--;
            if (lookingAtPlayer < 0)
                lookingAtPlayer = alivePlayers.Count - 1;

            mainCamera.Follow = alivePlayers[lookingAtPlayer].transform;
        }
    }

    public void SpectateAlive()
    {

        this.Invoke(() => {
            GameObject objective = alivePlayers.Find(go => go.activeSelf);
            if (objective != null)
                mainCamera.Follow = objective.transform;
        }, 1f);

        
    }
}
