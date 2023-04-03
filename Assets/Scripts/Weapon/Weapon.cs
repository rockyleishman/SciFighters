using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public Transform BarrelEnd;

    [SerializeField] public int Damage;
    [SerializeField] public float Inaccuracy;
    [SerializeField] public float Cooldown;
    internal float CooldownTimer { get; private protected set; }
    [SerializeField] public int MagazineSize;
    internal int Ammo { get; private protected set; }
    [SerializeField] public bool IsAutomatic;
    [SerializeField] public Color LaserColor;

    private void Start()
    {
        //gun initially loaded
        Ammo = MagazineSize;

        //init cooldown timer
        CooldownTimer = 0;
    }

    private void Update()
    {
        CooldownTimer -= Time.deltaTime;
    }

    public int Reload(int playerAmmo)
    {
        int neededAmmo = MagazineSize - Ammo;

        if (playerAmmo < neededAmmo)
        {
            Ammo = playerAmmo;
            playerAmmo = 0;
        }
        else
        {
            Ammo = neededAmmo;
            playerAmmo -= neededAmmo;
        }

        //////play reload sound

        return playerAmmo;
    }

    public void Fire(Unit user)
    {
        if (Ammo > 0 && CooldownTimer < 0.0f)
        {
            user.FireLaser(Damage, Inaccuracy, LaserColor);
            Ammo--;
            CooldownTimer = Cooldown;
        }
        else if (Cooldown < 0.0f)
        {
            //gun on cooldown
        }
        else
        {
            //////play dryfire sound
        }
    }
}