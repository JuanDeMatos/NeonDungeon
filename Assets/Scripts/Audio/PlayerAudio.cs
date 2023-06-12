using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{

    [SerializeField] AudioSource audioSourceDash;
    [SerializeField] AudioSource audioSourceShoot;

    public void PlayDash()
    {
        PlayDashServerRpc();
    }

    [ServerRpc]
    void PlayDashServerRpc()
    {
        PlayDashClientRpc();
    }

    [ClientRpc]
    void PlayDashClientRpc()
    {
        audioSourceDash.Play();
    }

}
