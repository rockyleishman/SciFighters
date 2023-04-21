using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityPowerup : Pickup
{
    [SerializeField] public float Duration = 15.0f;

    protected override void Collect(PlayerController player)
    {
        player.PowerupInvincibility(Duration);
    }
}
