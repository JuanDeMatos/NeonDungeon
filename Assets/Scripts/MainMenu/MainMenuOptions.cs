using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class MainMenuOptions : MonoBehaviour
{
    [SerializeField] TMP_InputField inputJoinCode;
    [SerializeField] RelayScript relayScript;
    public List<Button> buttons;
    public GameObject networkManagerSingleplayer;
    public GameObject networkManagerCoop;

    public bool dontUseRelay;

    private void Start() 
    {
        Debug.Log("Start");
        buttons = Resources.FindObjectsOfTypeAll<Button>().ToList();
        this.Invoke(() => buttons.RemoveAll(b => b == null),0.5f);
    }

    public void StartSinglePlayer()
    {
        Shared.gameMode = GameMode.Singleplayer;
        networkManagerSingleplayer.SetActive(true);
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("WaitingLobby",LoadSceneMode.Single);
    }

    public void CreateCoop()
    {
        
        Shared.gameMode = GameMode.Coop;
        

        if (dontUseRelay)
        {
            networkManagerSingleplayer.SetActive(true);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("WaitingLobby", LoadSceneMode.Single);
        } else
        {
            networkManagerCoop.SetActive(true);
            relayScript.CreateRelay();
        }
        
    }

    public void JoinCoop()
    {
        Shared.gameMode = GameMode.Coop;
        

        if (dontUseRelay)
        {
            networkManagerSingleplayer.SetActive(true);
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            networkManagerCoop.SetActive(true);
            relayScript.JoinRelay(inputJoinCode.text);
        }

        
    }

    public void EnableButtons()
    {
        buttons.ForEach(b => b.enabled = true);
    }

    public void DisableButtons()
    {
        buttons.ForEach(b => b.enabled = false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
