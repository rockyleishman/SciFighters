using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherit from the Gun class
public class AutomaticGunControl : Gun
{
    // Set the rate of fire
    public float fireRate = 0.2f;
    // Set the next available firing time
    private float nextFireTime = 0;

    // Override the Update method from the Gun class
    protected override void Update()
    {
        // Call the Update method from the Gun class
        base.Update();

        // Check if the Fire1 button is being held down and enough time has passed since the last shot
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            // Check if there is any ammo left
            if (currentAmmoBullets <= 0)
            {
                print("Need Reloading");
            }
            else
            {
                // Set the firing point and fire the gun
                firePoint = transform.position;
                Firing();
                // Decrement the ammo
                currentAmmoBullets--;
                Debug.Log("currentAmmoBullets:" + currentAmmoBullets);
                // Set the next available firing time
                nextFireTime = Time.time + fireRate;
            }
        }
        // Check if the Reload button is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reload");
            Reload();
        }
    }
}