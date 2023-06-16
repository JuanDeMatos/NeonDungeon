using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net.Mail;
using System;
using UnityEngine.Networking;

public class SignIn : MonoBehaviour
{
    [SerializeField] MainMenuOptions mainMenuOptions;
    [SerializeField]
    private TMP_InputField usernameInput, passwordInput, emailInput;
    [SerializeField]
    private TextMeshProUGUI errorText;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject signInMenu;
    public void Register()
    {
        if (usernameInput.text.Length == 0 || passwordInput.text.Length == 0 || emailInput.text.Length == 0)
            return;

        mainMenuOptions.DisableButtons();
        StartCoroutine(SignInCoroutine());
    }
    IEnumerator SignInCoroutine()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();
        string email = emailInput.text.Trim();

        string url = ServerProperties.BASEURL + "/register";
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        try
        {
            new MailAddress(email);
        }
        catch (Exception)
        {
            errorText.transform.parent.gameObject.SetActive(true);
            errorText.SetText("Invalid email address.");
            mainMenuOptions.EnableButtons();
            yield break;
        }

        UserDTO playerUser = new UserDTO(username, password, email);
        string json = JsonUtility.ToJson(playerUser);
        
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = certHandler;
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (www.responseCode == 409)
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.SetText("This user already exists.");
            }
            else
            {
                errorText.SetText(www.error);
            }
        }
        else
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                signInMenu.SetActive(false);
                Shared.logged = true;
                Shared.username = username;
                mainMenu.SetActive(true);
            }
            else
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.SetText(www.error);
                Debug.Log(www.responseCode + ";" + www.error);
            }
        }

        mainMenuOptions.EnableButtons();
    }
}

public struct UserDTO
{
    public string username;
    public string password;
    public string correo;

    public UserDTO(string username, string password, string email)
    {
        this.username = username;
        this.password = password;
        this.correo = email;
    }

    public override string ToString()
    {
        return username + ";" + password + ";" + correo;
    }

}
