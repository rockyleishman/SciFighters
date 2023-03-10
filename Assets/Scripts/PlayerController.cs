using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    private Camera _playerCamera; //for aiming
    private Transform _cameraPivot; //for moving view angle
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;


    private void Start()
    {
        base.Start();

        //player is always of the faction "player"
        UnitFaction = Faction.player;

        _playerCamera = GetComponentInChildren<Camera>();
        _cameraPivot = _playerCamera.transform.parent;
    }

    private void Update()
    {
        //rotate camera
        _cameraPivot.Rotate(-Input.GetAxis("Mouse Y") * MouseSensitivityY, 0, 0);

        //disallow other movement if dead
        if (!isAlive)
        {
            return;
        }

        //get movement input
        Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * MovementSpeed;

        //get jump input
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            movementInput.y = JumpVelocity;
            //////trigger jump animation
        }
        else
        {
            movementInput.y = _rigidbody.velocity.y;
        }

        //commit movement
        _rigidbody.velocity = transform.TransformVector(movementInput);
        //////animate movement

        //rotate player
        transform.Rotate(0, Input.GetAxis("Mouse X") * MouseSensitivityX, 0);

    }
}
