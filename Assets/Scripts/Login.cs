using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private string baseURL = "https://localhost:8443/users";
    public InputField usernameInput;
    public InputField passwordInput;

    private void Start()
    {
        SSLHelper.OverrideCertificateChainValidation();
        StartCoroutine(CargarDatosBaseCoroutine());
    }

    IEnumerator CargarDatosBaseCoroutine()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "/cargarDatosBase");
        www.certificateHandler = certHandler;
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string response = www.downloadHandler.text;
            Debug.Log("Datos iniciales cargados correctamente");
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    public void Log()
    {
        StartCoroutine(LoginCoroutine());
    }

    IEnumerator LoginCoroutine()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        string url = baseURL + "/login?username=" + UnityWebRequest.EscapeURL(usernameInput.text)
            + "&password=" + UnityWebRequest.EscapeURL(passwordInput.text);
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(""));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = certHandler;
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            int responseCode = (int)www.responseCode;

            if (responseCode == 200)
            {
                SceneManager.LoadScene("NuevaEscena");
            }
            else
            {
                Debug.Log("Inicio de sesión incorrecto.");
            }
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}