using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : Pickup
{
    [SerializeField] public float Duration = 30.0f;

    protected override void Collect(PlayerController player)
    {
        player.PowerupSpeed(Duration);
    }
}
