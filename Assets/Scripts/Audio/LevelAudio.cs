using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAudio : MonoBehaviour
{
    [SerializeField] private AudioClip fightTheme;
    [SerializeField] private AudioClip bossTheme;
    [SerializeField] private AudioClip bossDeadTheme;
    [SerializeField] private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        BossRoom.OnBossRoomStarted += BossRoom_OnBossRoomStarted;
        Boss.OnBossDeath += Boss_OnBossDeath;
        Room.OnRoomStarted += Room_OnRoomStarted;
    }

    private void Room_OnRoomStarted()
    {
        if (source.clip != fightTheme) {
            source.clip = fightTheme;
            source.Play();
        }

    }

    private void Boss_OnBossDeath()
    {
        source.clip = bossDeadTheme;
        source.Play();
    }

    private void BossRoom_OnBossRoomStarted()
    {
        source.clip = bossTheme;
        source.Play();
    }

    private void OnDestroy()
    {
        BossRoom.OnBossRoomStarted -= BossRoom_OnBossRoomStarted;
        Boss.OnBossDeath -= Boss_OnBossDeath;
        Room.OnRoomStarted -= Room_OnRoomStarted;
    }

}
