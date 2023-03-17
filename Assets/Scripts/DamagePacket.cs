using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePacket : MonoBehaviour
{
    //used to deliver damage data to hit targets

    internal int DamageAmount { get; private set; }
    internal Faction DamageFrom { get; private set; }

    internal DamagePacket(int damageAmount, Faction damageFrom)
    {
        DamageAmount = damageAmount;
        DamageFrom = damageFrom;
    }
}
