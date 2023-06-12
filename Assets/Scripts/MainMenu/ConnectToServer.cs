using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject noConnectionPanel;
    [SerializeField] GameObject dailyRunButton;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject loadingTitle, loadedTitle;

    // Start is called before the first frame update
    void Start()
    {
        switch (Shared.connectionState)
        {
            case ConnectionState.DEFAULT:
                UnityWebRequest.ClearCookieCache();
                StartCoroutine(GetTodaySeed());
                SSLHelper.OverrideCertificateChainValidation();
                break;
            case ConnectionState.YES:
                Connection();
                break;
            case ConnectionState.NO:
                NoConnection();
                break;
        }
    }
    IEnumerator GetTodaySeed()
    {
        MyCertificateHandler certHandler = new MyCertificateHandler();
        string url = ServerProperties.BASEURL + "/dailyrunseed";

        Debug.Log(url);

        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(""));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = certHandler;
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Connection();
            Shared.dailyRunSeed = int.Parse(www.downloadHandler.text);
            Debug.Log(Shared.dailyRunSeed);
        }
        else
        {
            NoConnection();
        }
    }

    private void NoConnection()
    {
        Shared.connectionState = ConnectionState.NO;
        noConnectionPanel.SetActive(true);
        dailyRunButton.SetActive(false);
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        loadingTitle.SetActive(false);
        loadedTitle.SetActive(true);
    }

    private void Connection()
    {
        Debug.Log("Connection");
        Shared.connectionState = ConnectionState.YES;
        noConnectionPanel.SetActive(false);

        if (!Shared.logged)
        {
            loginPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        } else
        {
            loginPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }

        dailyRunButton.SetActive(true);
        loadingTitle.SetActive(false);
        loadedTitle.SetActive(true);
    }
}
