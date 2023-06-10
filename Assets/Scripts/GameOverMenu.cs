using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{

    [SerializeField] private GameObject winText,loseText;
    [SerializeField] private TextMeshProUGUI scoreText, hitsText, timeText;

    void Start()
    {
        SSLHelper.OverrideCertificateChainValidation();

        if (RunScore.time > 0)
            StartCoroutine(SendRunScoreToBackend());

        EvaluateWinOrLose();
        SetGameOverTexts();
    }

    IEnumerator SendRunScoreToBackend()
    {
        UnityWebRequest www = new UnityWebRequest(ServerProperties.BASEURL + "/saverun","POST");

        RunDTO runDTO = new RunDTO(RunScore.win,(int) RunScore.time,(int) RunScore.score, RunScore.hits,Shared.gameMode);

        string json = JsonUtility.ToJson(runDTO);

        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.certificateHandler = new MyCertificateHandler();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }

    private void EvaluateWinOrLose()
    {
        
         if (RunScore.win)
         {
             winText.SetActive(true);
         }
         else
         {
             loseText.SetActive(true);
         }
        
    }

    private void SetGameOverTexts()
    {
        scoreText.SetText(((int)RunScore.score) + "");
        hitsText.SetText(RunScore.hits + "");

        int time = (int)RunScore.time;

        string seconds = time % 60 + "";
        seconds = seconds.Length < 2 ? "0" + seconds : seconds;

        string minutes = time / 60 + "";
        minutes = minutes.Length < 2 ? "0" + minutes : minutes;

        timeText.SetText(minutes + ":" + seconds);
    }

    public void GoBackToMainMenu()
    {

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

}

public struct RunDTO
{
    public bool ganada;
    public int tiempo;
    public int puntuacion;
    public int hits;
    public string tipo;

    public RunDTO(bool ganada,int tiempo,int puntuacion, int hits, GameMode tipo)
    {
        this.ganada = ganada;
        this.tiempo = tiempo;
        this.puntuacion = puntuacion;
        this.hits = hits;

        switch (tipo)
        {
            case GameMode.DailyRun:
                this.tipo = "DAILY";
                break;
            case GameMode.Coop:
                this.tipo = "COOP";
                break;
            default:
                this.tipo = "SOLO";
                break;
        }
    }
}
