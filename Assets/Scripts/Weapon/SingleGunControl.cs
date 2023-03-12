using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define SingleGunControl class that inherits from Gun class
public class SingleGunControl : Gun
{
    // Override Start method to call parent's Start method
    public override void Start()
    {
        base.Start();
    }

    // Override Update method to check for user input
    protected override void Update()
    {
        base.Update();

        // Check if left mouse button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            // Check if there are any bullets left
            if (currentAmmoBullets <= 0)
            {
                print("Need Reloading");
            }
            else
            {
                firePoint = transform.position;
                Firing();
                currentAmmoBullets--;
                Debug.Log("currentAmmoBullets:" + currentAmmoBullets);
            }
        }

        // Check if 'R' key is pressed to reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reload");
            Reload();
        }
    }
}