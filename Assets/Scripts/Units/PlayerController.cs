using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    private Camera _playerCamera; //for aiming
    private Transform _cameraPivot; //for moving view angle
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;

    protected bool _isSprinting = false;
    protected bool _isCrouching = false;
    protected bool _isSliding = false;
    protected bool _isSlideCooling = false;
    private bool _isJumping = false;

    protected override void Start()
    {
        base.Start();

        //player is always of the faction "player"
        UnitFaction = Faction.player;

        //player has no forced inaccuracy or trigger delay
        TriggerDelay = 0.0f;
        MaxInaccuracyDegrees = 0.0f;

        _playerCamera = GetComponentInChildren<Camera>();
        _cameraPivot = _playerCamera.transform.parent;
    }

    private void Update()
    {
        //rotate camera
        _cameraPivot.Rotate(-Input.GetAxis("Mouse Y") * MouseSensitivityY, 0, 0);

        //disallow other movement if dead
        if (!IsAlive)
        {
            return;
        }

        //update isJumping
        if (IsGrounded())
        {
            _isJumping = false;
        }

        //get slide input
        if (Input.GetKeyDown(KeyCode.LeftControl) && !_isSlideCooling && (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f))
        {
            _isSliding = true;
            _isSlideCooling = true;
            _slideTimer = 0.0f;
        }

        //determine if slide can be executed next frame
        _slideTimer += Time.deltaTime;
        if (_slideTimer >= SlideTime)
        {
            _isSliding = false;
        }
        if (_slideTimer >= SlideCooldown)
        {
            _isSlideCooling = false;
        }

        //get crouch/sprint input
        if (Input.GetKey(KeyCode.LeftControl) && !_isJumping)
        {
            if (_isSliding)
            {
                _currentSpeed = SlideSpeed;
            }
            else
            {
                _currentSpeed = CrouchSpeed;
            }
            _isCrouching = true;
            _isSprinting = false;

            //////crouch camera & hitbox
        }
        else if (Input.GetKey(KeyCode.LeftShift) && !_isJumping)
        {
            if (Input.GetKey(KeyCode.W))   // Only can run when press W key
            {
                _currentSpeed = SprintSpeed;
                _isSprinting = true;
                _isCrouching = false;
                _isSliding = false;
            }
            

        }
        else
        {
            _currentSpeed = MovementSpeed;
            _isSprinting = false;
            _isCrouching = false;
            _isSliding = false;
        }

        //change collider height for crouching
        /*
        if (_isCrouching)
        {
            UpdateHitBoxHeight(CrouchHeightRatio);
        }
        else
        {
            UpdateHitBoxHeight(1.0f);
        }
        */

        //switch weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            _equipedWeaponSlot++;
            if (_equipedWeaponSlot >= Weapons.Length)
            {
                _equipedWeaponSlot = 0;
            }

            _equipedWeapon = Weapons[_equipedWeaponSlot];
            _gunTip = _equipedWeapon.BarrelEnd;

            //////play weapon change sound
            //////visual weapon change needed
        }

        //reload weapon
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnloadedAmmo = _equipedWeapon.Reload(UnloadedAmmo);
        }

        //fire weapon
        if (Input.GetMouseButton(0) && _equipedWeapon.IsAutomatic)
        {
            _equipedWeapon.Fire(this);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            _equipedWeapon.Fire(this);
        }

        //get movement input
        Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _currentSpeed;


            //get jump input
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _isJumping = true;
            movementInput.y = JumpVelocity;

            ////////TEMP FIX UNTIL MENU BUILT, REMOVE LATER
            //remove cursor
            Cursor.lockState = CursorLockMode.Locked;
            ////
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
