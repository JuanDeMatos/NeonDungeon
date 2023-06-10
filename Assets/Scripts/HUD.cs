using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using Unity.Netcode;

public class HUD : MonoBehaviour
{
    private SharedInventory sharedInventory;
    public Player localPlayer;

    [Header("Consumables")]
    [SerializeField] TextMeshProUGUI textKeys;
    [SerializeField] TextMeshProUGUI textCoins;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI textMovementSpeed;
    [SerializeField] TextMeshProUGUI textInvulnerable;
    [SerializeField] TextMeshProUGUI textDashSpeed;
    [SerializeField] TextMeshProUGUI textDashRange;
    [SerializeField] TextMeshProUGUI textDamage;
    [SerializeField] TextMeshProUGUI textShootSpeed;
    [SerializeField] TextMeshProUGUI textBulletSpeed;
    [SerializeField] TextMeshProUGUI textRange;

    [Header("Dashes")]
    [SerializeField] Color fullChargedColor;
    [SerializeField] Color emptyChargeColor;
    [SerializeField] TextMeshProUGUI textDashes;
    [SerializeField] Image dashChargingImage;

    [Header("Health Bar")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI textHealth;

    [Header("Waiting")]
    private bool waitingPanelActive;
    [SerializeField] GameObject waitingPanel;
    [SerializeField] TextMeshProUGUI waitingText;
    [SerializeField] TextMeshProUGUI waitingTextPlayersReady;
    [SerializeField] Image waitingImage;

    [Header("Entra Info")]
    [SerializeField] GameObject extraInfo;
    [SerializeField] TextMeshProUGUI score;
    [SerializeField] TextMeshProUGUI hits;
    [SerializeField] TextMeshProUGUI time;


    // Start is called before the first frame update
    void Start()
    {
        sharedInventory = FindObjectOfType<SharedInventory>();
        localPlayer = FindObjectsOfType<Player>().ToList().Find(p => p.IsLocalPlayer);
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HideWaitingPanel;
    }

    private void HideWaitingPanel(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        HideWaitingPanel();
    }

    private void Update()
    {
        Consumables();
        Stats();
        Dashes();
        HealthBar();
        Score();

    }

    private void Score()
    {
        score.SetText(((int)RunScore.score) + "");
        hits.SetText(RunScore.hits + "");

        int time = (int)RunScore.time;

        string seconds = time % 60 + "";
        seconds = seconds.Length < 2 ? "0" + seconds : seconds;

        string minutes = time / 60 + "";
        minutes = minutes.Length < 2 ? "0" + minutes : minutes;

        this.time.SetText(minutes + ":" + seconds);
    }

    private void Consumables()
    {
        textKeys.SetText("" + sharedInventory.keys.Value);
        textCoins.SetText("" + sharedInventory.coins.Value);
    }

    private void Stats()
    {
        textMovementSpeed.SetText(FormatFloat(localPlayer.movementSpeed));
        textInvulnerable.SetText(FormatFloat(localPlayer.invulnerableTime));
        textDashSpeed.SetText(FormatFloat(localPlayer.dashSpeed));
        textDashRange.SetText(FormatFloat(localPlayer.dashRange));
        textDamage.SetText(FormatFloat(localPlayer.damage));
        textShootSpeed.SetText(FormatFloat(localPlayer.shootSpeed));
        textBulletSpeed.SetText(FormatFloat(localPlayer.bulletSpeed));
        textRange.SetText(FormatFloat(localPlayer.range));
    }

    private void HealthBar()
    {
        int maxHealth = (int)localPlayer.maxHealth;
        int health = (int)localPlayer.health;

        textHealth.SetText(health + " / " + maxHealth);

        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    private void Dashes()
    {
        float dashAcumulator = localPlayer.dashChargesAcumulator;
        textDashes.SetText((int)dashAcumulator + "");

        float h1, s1, v1, h2, s2, v2, h3, s3, v3;

        Color.RGBToHSV(emptyChargeColor, out h1, out s1, out v1);
        Color.RGBToHSV(fullChargedColor, out h2, out s2, out v2);
        float t = dashAcumulator / localPlayer.dashCharges;

        h3 = Mathf.Lerp(h1, h2, t);
        s3 = Mathf.Lerp(s1, s2, t);
        v3 = Mathf.Lerp(v1, v2, t);

        dashChargingImage.fillAmount = dashAcumulator > localPlayer.dashCharges ? 1 : dashAcumulator % 1;
        dashChargingImage.color = Color.HSVToRGB(h3, s3, v3);
    }

    private string FormatFloat(float value)
    {
        decimal decimalValue = (decimal) value;
        string formatted = decimal.Round(decimalValue, 2).ToString();

        if (formatted == "0")
            return formatted;

        if (formatted.EndsWith(".00"))
        {
            formatted = formatted.Substring(0, formatted.Length - 3);
        } else if (formatted.EndsWith("0")) {
            formatted = formatted.Substring(0, formatted.Length - 1);
        }

        return formatted;
    }

    public void ShowWaitingPanel(Sprite sprite,int playersReady, string text, bool showPlayersReady = true)
    {
        waitingImage.sprite = sprite;
        waitingText.SetText(text);

        if (showPlayersReady)
        {
            waitingTextPlayersReady.SetText($"{playersReady} / {LooseState.alivePlayers}");
        }
        else
        {
            waitingTextPlayersReady.SetText("");
        }

        waitingPanelActive = true;
        waitingPanel.SetActive(true);
    }

    public void HideWaitingPanel()
    {
        waitingPanelActive = false;
        waitingPanel.SetActive(false);
    }

    public void BigMinimap(bool show)
    {
        extraInfo.SetActive(show);
        waitingPanel.SetActive(waitingPanelActive && !show);
    }

}