using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject HUDCanvas, pauseCanvas;
    [SerializeField] GameObject qualityDropdown;
    public static bool paused = false;

    private void Start()
    {
        paused = false;
        Time.timeScale = 1;
    }

    public void Pause()
    {
        FindObjectOfType<EventSystem>().SetSelectedGameObject(qualityDropdown);

        paused = !paused;

        HUDCanvas.SetActive(!paused);
        pauseCanvas.SetActive(paused);

        if (Shared.gameMode != GameMode.Coop)
        {
            Time.timeScale = paused ? 0 : 1;
        }
    }

    public void Exit()
    {
        paused = false;
        Time.timeScale = 1;
        FindObjectOfType<LooseState>().Shutdown(false);
    }
}
