using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : Pickup
{
    [SerializeField] public int MinAmmoAmount = 10;
    [SerializeField] public int MaxAmmoAmount = 10;

    protected override void Collect(PlayerController player)
    {
        player.AddAmmo(Random.Range(MinAmmoAmount, MaxAmmoAmount + 1));

        UIManager.Instance.UpdateAmmo();
    }
}
