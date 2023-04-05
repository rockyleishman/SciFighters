using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public Transform BarrelEnd;
    [SerializeField] public Audio ReloadAudioPrefab;
    [SerializeField] public Audio FireAudioPrefab;
    [SerializeField] public Audio DryFireAudioPrefab;

    [SerializeField] public int Damage;
    [SerializeField] public float Inaccuracy;
    [SerializeField] public float Cooldown;
    internal float CooldownTimer { get; private protected set; }
    [SerializeField] public int MagazineSize;
    internal int Ammo { get; private protected set; }
    [SerializeField] public float ReloadTime;
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

        if (neededAmmo == 0)
        { 
            //gun is already fully loaded
        }
        else if (playerAmmo < neededAmmo)
        {
            Ammo = playerAmmo;
            playerAmmo = 0;

            CooldownTimer = ReloadTime;

            Instantiate(ReloadAudioPrefab);
        }
        else
        {
            Ammo += neededAmmo;
            playerAmmo -= neededAmmo;

            CooldownTimer = ReloadTime;

            Instantiate(ReloadAudioPrefab);
        }

        return playerAmmo;
    }

    //attempts to fire the gun and returns true if the gun was loaded
    public bool Fire(Unit user)
    {
        if (Ammo > 0 && CooldownTimer < 0.0f)
        {
            user.FireLaser(Damage, Inaccuracy, LaserColor);
            Ammo--;
            CooldownTimer = Cooldown;

            Instantiate(FireAudioPrefab);
        }
        else if (CooldownTimer > 0.0f)
        {
            //gun on cooldown
        }
        else
        {
            //dry fire
            CooldownTimer = Cooldown;

            Instantiate(DryFireAudioPrefab);
            
            return false;
        }

        return true;
    }
}