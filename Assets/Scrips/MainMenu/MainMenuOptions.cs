using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuOptions : MonoBehaviour
{
    [SerializeField] TMP_InputField inputJoinCode;
    [SerializeField] RelayScript relayScript;

    public void CreateCoop()
    {
        relayScript.CreateRelay();
    }

    public void JoinCoop()
    {
        relayScript.JoinRelay(inputJoinCode.text);
    }


}
