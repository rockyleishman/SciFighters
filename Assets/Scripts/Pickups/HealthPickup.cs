using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickup
{
    [SerializeField] public int MinHealAmount = 10;
    [SerializeField] public int MaxHealAmount = 10;

    protected override void Collect(PlayerController player)
    {
        player.Heal(Random.Range(MinHealAmount, MaxHealAmount + 1));
    }
}
