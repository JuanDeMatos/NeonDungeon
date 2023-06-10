using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RelayScript : NetworkBehaviour
{
    public int MAXPLAYERS;
    public string playerID;
    public string joinCode;
    public MainMenuOptions mainMenuOptions;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            playerID = AuthenticationService.Instance.PlayerId;
            Debug.Log("Loged In: " + playerID);
        };

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void CreateRelay()
    {
        mainMenuOptions.DisableButtons();
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAXPLAYERS);

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Shared.joinCode = joinCode;

            Debug.Log("Join code: " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation,"dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            NetworkManager.SceneManager.LoadScene("WaitingLobby", LoadSceneMode.Single);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        mainMenuOptions.EnableButtons();
    }

    public async void JoinRelay(string joinCode)
    {
        mainMenuOptions.DisableButtons();
        try
        {
            Debug.Log("Joining Relay with code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Shared.joinCode = joinCode;

            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
        mainMenuOptions.EnableButtons();
    }
}
