using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlimitedAmmoPowerup : Pickup
{
    [SerializeField] public float Duration = 15.0f;

    protected override void Collect(PlayerController player)
    {
        player.PowerupUnlimitedAmmo(Duration);
    }
}
