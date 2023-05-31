using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Item : NetworkBehaviour, IEquatable<Item>
{
    [Header("Item Attributes")]
    public string itemName;
    public string description;

    [Header("Stat Modifiers")]
    [Header("Player Attributes")]
    public int dashCharges;
    public Stat health;
    public Stat movementSpeed;
    public Stat invulnerableTime;
    public Stat dashSpeed;
    public Stat dashRange;

    [Header("Bullet Properties")]
    public Stat shootSpeed;
    public Stat damage;
    public Stat bulletSpeed;
    public Stat range;

    public void ApplyItem(Player player)
    {
        if (player.dashCharges + dashCharges > 0)
            player.dashCharges += dashCharges;
        else
            player.dashCharges = 1;

        ApplyStat(ref player.health, health,500,1);
        ApplyStat(ref player.movementSpeed, movementSpeed,30,5);
        ApplyStat(ref player.invulnerableTime, invulnerableTime,5,0.5f);
        ApplyStat(ref player.dashSpeed, dashSpeed,20,3);
        ApplyStat(ref player.dashRange, dashRange,3,1);
        if (Shared.gameMode == GameMode.Coop)
            ApplyStat(ref player.shootSpeed, shootSpeed, 100, 0.5f);
        else
            ApplyStat(ref player.shootSpeed, shootSpeed, 100, 0.1f);
        ApplyStat(ref player.damage, damage,99999,1);
        ApplyStat(ref player.bulletSpeed, bulletSpeed,50,5);
        ApplyStat(ref player.range, range,0,30);

    }
    public void ClearItem(Player player)
    {
        if (player.dashCharges - dashCharges > 0)
            player.dashCharges -= dashCharges;
        else
            player.dashCharges = 1;

        ClearStat(ref player.health, health,500,1);
        ClearStat(ref player.movementSpeed, movementSpeed,30,5);
        ClearStat(ref player.invulnerableTime, invulnerableTime,5,0.5f);
        ClearStat(ref player.dashSpeed, dashSpeed, 20,3);
        ClearStat(ref player.dashRange, dashRange,3,1);
        if (Shared.gameMode == GameMode.Coop)
            ClearStat(ref player.shootSpeed, shootSpeed, 100, 0.5f);
        else
            ClearStat(ref player.shootSpeed, shootSpeed, 100, 0.1f);
        ClearStat(ref player.damage, damage,99999,1);
        ClearStat(ref player.bulletSpeed, bulletSpeed,50,5);
        ClearStat(ref player.range, range,0,30);
    }

    private void ApplyStat(ref float playerStat, Stat itemStat, float maxLimit, float minLimit)
    {
        float value = playerStat;

        if (itemStat.isMultiplier)
            value *= itemStat.value == 0 ? 1 : itemStat.value;
        else
            value += itemStat.value;

        if (value > maxLimit && value >= minLimit)
            playerStat = maxLimit;
        else if (value < minLimit)
            playerStat = minLimit;
        else
            playerStat = value;
            
    }

    private void ClearStat(ref float playerStat, Stat itemStat, float maxLimit, float minLimit)
    {
        float value = playerStat;

        if (itemStat.isMultiplier)
            value /= itemStat.value == 0 ? 1 : itemStat.value;
        else
            value -= itemStat.value;

        if (value > maxLimit && value >= minLimit)
            playerStat = maxLimit;
        else if (value < minLimit)
            playerStat = minLimit;
        else
            playerStat = value;
    }


    public override bool Equals(object obj)
    {
        return Equals(obj as Item);
    }

    public bool Equals(Item other)
    {
        return other != null &&
               base.Equals(other) &&
               itemName == other.itemName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), itemName);
    }


    [System.Serializable]
    public class Stat
    {
        public bool isMultiplier;
        public float value;
    }

}


