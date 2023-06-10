using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LogIn : MonoBehaviour
{
    [SerializeField] MainMenuOptions mainMenuOptions;
    [SerializeField]
    private TMP_InputField usernameInput;
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private TextMeshProUGUI errorText;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject loginMenu;    

    private void Start()
    {
        if (!Shared.logged)
        {
            UnityWebRequest.ClearCookieCache();
            SSLHelper.OverrideCertificateChainValidation();
        } else
        {
            loginMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void Login()
    {
        mainMenuOptions.DisableButtons();
        StartCoroutine(LogInCoroutine());
    }

    public void SignIn()
    {
        mainMenuOptions.DisableButtons();
        StartCoroutine(SignInCoroutine());
    }
    /*
    public void PruebaCookie()
    {
        StartCoroutine(PruebaCookieCoroutine());
    }

    IEnumerator PruebaCookieCoroutine()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        string url = baseURL + "/prueba";

        Debug.Log(url);

        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(""));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = certHandler;
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.downloadHandler.text);
            yield break;
        }


        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(www.downloadHandler.text);
            yield break;
        }

        Debug.Log(www.error);
    }
    */
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

    IEnumerator SignInCoroutine()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        string username = UnityWebRequest.EscapeURL(usernameInput.text);
        string password = UnityWebRequest.EscapeURL(passwordInput.text);
        string url = ServerProperties.BASEURL + "/register?username=" + username + "&password=" + password;

        Debug.Log(url);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(""));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = certHandler;
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError)
        {
            if (www.responseCode == 409)
            {
                errorText.transform.parent.gameObject.SetActive(true);
                errorText.SetText("This user already exists.");
            } else
            {
                errorText.SetText(www.error);
            }
        } else
        {
            if (www.result == UnityWebRequest.Result.Success)
            {
                loginMenu.SetActive(false);
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

