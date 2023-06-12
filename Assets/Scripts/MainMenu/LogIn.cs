using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Net.Mail;
using System;

public class LogIn : MonoBehaviour
{
    [SerializeField] MainMenuOptions mainMenuOptions;
    [SerializeField]
    private TMP_InputField usernameInput, passwordInput;
    [SerializeField]
    private TextMeshProUGUI errorText;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject loginMenu;    

    public void Login()
    {
        if (usernameInput.text.Length == 0 || passwordInput.text.Length == 0)
            return;

        mainMenuOptions.DisableButtons();
        StartCoroutine(LogInCoroutine());
    }



    IEnumerator LogInCoroutine()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        string username = UnityWebRequest.EscapeURL(usernameInput.text);
        string password = UnityWebRequest.EscapeURL(passwordInput.text);
        string url = ServerProperties.BASEURL + "/login?username=" + username + "&password=" + password;

        Debug.Log(url);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(""));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = certHandler;
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (www.responseCode == 403)
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.SetText("Wrong username or password.");
            } else
            {
                errorText.SetText(www.downloadHandler.text);
            }
        } else
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                loginMenu.SetActive(false);
                Shared.username = username;
                mainMenu.SetActive(true);
            } else
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.SetText(www.error);
                Debug.Log(www.responseCode + ";" + www.error);
            }
        }

        Shared.logged = true;
        mainMenuOptions.EnableButtons();
    }


}



