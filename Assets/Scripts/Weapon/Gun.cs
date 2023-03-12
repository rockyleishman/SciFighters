using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting; // Import Visual Scripting package
using UnityEngine;
using static UnityEditor.Progress; // Import Unity Editor Progress class

[RequireComponent(typeof(AudioSource))] // Require an AudioSource component on this game object
[RequireComponent(typeof(AudioSource))] // Require another AudioSource component on this game object
public class Gun : MonoBehaviour
{
    protected int ammoCapacity = 5; // Maximum capacity of bullets in the gun
    protected int currentAmmoBullets = 5; // Current number of bullets in the gun
    protected int remainBullets = 10; // Remaining bullets not in the gun
    protected Vector3 firePoint; // Position to spawn bullets from when firing

    protected AudioClip fireClip; // Audio clip for firing sound
    protected AudioClip reloadClip; // Audio clip for reload sound
    protected AudioSource fireSource; // AudioSource for firing sound
    protected AudioSource reloadSource; // AudioSource for reload sound

    public virtual void Start()
    {
        var as_array = this.gameObject.GetComponents(typeof(AudioSource)); // Get all AudioSources attached to this game object
        fireSource = (AudioSource)as_array[0]; // Set the first AudioSource to fireSource
        reloadSource = (AudioSource)as_array[1]; // Set the second AudioSource to reloadSource
        fireClip = fireSource.clip; // Set the firing audio clip
        reloadClip = reloadSource.clip; // Set the reload audio clip
    }

    protected int count = 1; // Unused integer variable
    protected virtual void Update()
    {
        // Update method that currently does nothing
    }

    /// <summary>
    /// Reload the gun with bullets
    /// </summary>
    protected void Reload()
    {
        reloadSource.PlayOneShot(reloadClip); // Play the reload sound

        if (remainBullets <= 0) // If there are no remaining bullets
        {
            Debug.Log("Out of Ammo"); // Print a message to the console
        }
        else // If there are remaining bullets
        {
            remainBullets = remainBullets - (ammoCapacity - currentAmmoBullets); // Subtract the bullets needed to fill the gun from the remaining bullets
            if (remainBullets / ammoCapacity > 0) // If there are enough remaining bullets to fill the gun
            {
                currentAmmoBullets = ammoCapacity; // Set the current bullets in the gun to the maximum capacity
                Debug.Log("currentAmmoBullets:" + currentAmmoBullets); // Print a message to the console
            }
            else // If there are not enough remaining bullets to fill the gun
            {
                currentAmmoBullets = remainBullets; // Set the current bullets in the gun to the remaining bullets
                Debug.Log("currentAmmoBullets:" + currentAmmoBullets); // Print a message to the console
            }
        }
        Debug.Log("remainBullets:" + remainBullets); // Print a message to the console with the number of remaining bullets
    }

    /// <summary>
    /// Fire the gun
    /// </summary>
    protected void Firing()
    {
        fireSource.PlayOneShot(fireClip); // Play the firing sound
    }
}