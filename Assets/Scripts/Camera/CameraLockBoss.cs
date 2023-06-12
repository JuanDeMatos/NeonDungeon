using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLockBoss : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera mainCamera;
    GameObject targetGroup;
    Transform player;

    private void Start()
    {
        Boss.OnBossSpawned += Boss_OnBossSpawned;
        Boss.OnBossDeath += Boss_OnBossDeath;
    }

    private void Boss_OnBossDeath()
    {
        mainCamera.Follow = player;
    }

    private void Boss_OnBossSpawned(Transform boss)
    {
        LockBoss(boss);
    }

    public void LockBoss(Transform boss)
    {
        player = mainCamera.Follow;
        targetGroup = new GameObject("TargetGroup");
        CinemachineTargetGroup tg = targetGroup.AddComponent<CinemachineTargetGroup>();

        tg.AddMember(player, 1, 10);
        tg.AddMember(boss, 1, 10);

        mainCamera.Follow = targetGroup.transform;
    }

}
