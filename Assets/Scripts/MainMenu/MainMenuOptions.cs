using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class MainMenuOptions : MonoBehaviour
{
    [SerializeField] TMP_InputField inputJoinCode;
    [SerializeField] RelayScript relayScript;
    private List<Button> buttons;

    private void Start()
    {
        buttons = Resources.FindObjectsOfTypeAll<Button>().ToList();

    }

    public void CreateCoop()
    {
        relayScript.CreateRelay();
    }

    public void JoinCoop()
    {
        relayScript.JoinRelay(inputJoinCode.text);
    }

    public void EnableButtons()
    {
        buttons.ForEach(b => b.enabled = true);
    }

    public void DisableButtons()
    {
        buttons.ForEach(b => b.enabled = false);
    }
}
