using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaitForPlayers : MonoBehaviour
{
    [SerializeField] private TextMeshPro playersReadyText;
    [SerializeField] private Sprite HUDSprite;
    [SerializeField] private string waitingText;
    [SerializeField] private string countdownStartedText;
    public int numberPlayersReady = 0;

    public delegate void AllPlayersReadyHandler();
    public event AllPlayersReadyHandler OnAllPlayersReady;
    [SerializeField] int playersReadyCountdown;

    private bool active;
    private HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        this.Invoke(() => {
            active = true;
            hud = FindObjectOfType<HUD>();
        }, 1f);
    }

    void Update()
    {
        if (playersReadyText != null)
        {
            playersReadyText.transform.eulerAngles = new Vector3(60, 0, 0);

            playersReadyText.SetText(numberPlayersReady + " / " + LooseState.alivePlayers);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active)
            return;

        if (!other.CompareTag("Player"))
            return;

        numberPlayersReady++;
        FindObjectOfType<HUD>().SetWaitingPanel(HUDSprite, numberPlayersReady, waitingText);

        if (numberPlayersReady == LooseState.alivePlayers)
        {

            StartCoroutine(PlayersReadyTimeout());

        }

    }

    IEnumerator PlayersReadyTimeout()
    {
        for (int i = playersReadyCountdown; i > 0; i--)
        {
            hud.SetWaitingPanel(HUDSprite, numberPlayersReady, $"{countdownStartedText} in {i}...", false);
            yield return new WaitForSeconds(1);
        }
        this.gameObject.SetActive(false);
        hud?.HideWaitingPanel();
        OnAllPlayersReady();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!active)
            return;

        if (!other.CompareTag("Player"))
            return;

        StopAllCoroutines();

        numberPlayersReady--;

        if (numberPlayersReady <= 0)
        {
            hud.HideWaitingPanel();
        } else
        {
            hud.SetWaitingPanel(HUDSprite, numberPlayersReady, "Waiting...");
        }
    }
}
