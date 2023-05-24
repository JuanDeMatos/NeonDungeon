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
    public Stat health;
    public Stat movementSpeed;
    public Stat invulnerableTime;
    public Stat dashSpeed;
    public Stat dashDuration;

    [Header("Bullet Properties")]
    public Stat damage;
    public Stat bulletSpeed;
    public Stat range;

    public void ApplyItem(Player player)
    {
        ApplyStat(ref player.health, health);
        ApplyStat(ref player.movementSpeed, movementSpeed);
        ApplyStat(ref player.invulnerableTime, invulnerableTime);
        ApplyStat(ref player.dashSpeed, dashSpeed);
        ApplyStat(ref player.dashDuration, dashDuration);
        ApplyStat(ref player.damage, damage);
        ApplyStat(ref player.bulletSpeed, bulletSpeed);
        ApplyStat(ref player.range, range);

    }
    public void ClearItem(Player player)
    {
        ClearStat(ref player.health, health);
        ClearStat(ref player.movementSpeed, movementSpeed);
        ClearStat(ref player.invulnerableTime, invulnerableTime);
        ClearStat(ref player.dashSpeed, dashSpeed);
        ClearStat(ref player.dashDuration, dashDuration);
        ClearStat(ref player.damage, damage);
        ClearStat(ref player.bulletSpeed, bulletSpeed);
        ClearStat(ref player.range, range);
    }

    private void ApplyStat(ref float playerStat, Stat itemStat)
    {
        if (itemStat.isMultiplier)
            playerStat *= itemStat.value == 0 ? 1 : itemStat.value;
        else
            playerStat += itemStat.value;
    }

    private void ClearStat(ref float playerStat, Stat itemStat)
    {
        if (itemStat.isMultiplier)
            playerStat /= itemStat.value == 0 ? 1 : itemStat.value;
        else
            playerStat -= itemStat.value;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject playerGameObject = other.gameObject;
            Player player = playerGameObject.GetComponent<Player>();
            player.AddItem(this);

            if (IsServer)
                GetComponent<NetworkObject>().Despawn();
            //Destroy(this.gameObject);
        }
    }


    [System.Serializable]
    public class Stat
    {
        public bool isMultiplier;
        public float value;
    }

}


