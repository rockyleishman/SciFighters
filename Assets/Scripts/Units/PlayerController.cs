using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    private Camera _playerCamera; //for aiming
    private Transform _cameraPivot; //for moving view angle
    private Vector3 _cameraPivotStandingPosition;
    private Vector3 _cameraPivotCrouchingPosition;
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;

    [SerializeField] public Audio WeaponChangeAudioPrefab;

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

        //init camera and pivot
        _playerCamera = GetComponentInChildren<Camera>();
        _cameraPivot = _playerCamera.transform.parent;

        //get camera positions
        _cameraPivotStandingPosition = _cameraPivot.localPosition;
        _cameraPivotCrouchingPosition = new Vector3(_cameraPivotStandingPosition.x, _cameraPivotStandingPosition.y * CrouchHeightRatio, _cameraPivotStandingPosition.z);

        //hide weapons
        foreach (Weapon gun in Weapons)
        {
            gun.transform.localScale = Vector3.zero;
        }
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

        //toggle crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (_isCrouching)
            {
                _isCrouching = false;
                _isSliding = false;
            }
            else
            {
                _isCrouching = true;

                //get slide input
                if (!_isSlideCooling && (Input.GetAxisRaw("Horizontal") != 0.0f || Input.GetAxisRaw("Vertical") != 0.0f))
                {
                    _isSliding = true;
                    _isSlideCooling = true;
                    _slideTimer = 0.0f;
                }
            }
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
        if (_isCrouching)
        {
            if (_isSliding)
            {
                _currentSpeed = SlideSpeed;
            }
            else
            {
                _currentSpeed = CrouchSpeed;
            }
            _isSprinting = false;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && !_isJumping)
        {
            //can only run forward and forward-diagonal
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
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

        //change camera height for crouching
        if (_isCrouching)
        {
            _cameraPivot.localPosition = Vector3.MoveTowards(_cameraPivot.localPosition, _cameraPivotCrouchingPosition, Time.deltaTime * CrouchCameraSpeed);
        }
        else
        {
            _cameraPivot.localPosition = Vector3.MoveTowards(_cameraPivot.localPosition, _cameraPivotStandingPosition, Time.deltaTime * CrouchCameraSpeed);
        }

        //switch weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            _equipedWeaponSlot++;
            if (_equipedWeaponSlot >= Weapons.Length)
            {
                _equipedWeaponSlot = 0;
            }

            EquipWeapon();

            Audio sound = Instantiate(WeaponChangeAudioPrefab);
            sound.transform.position = transform.position;
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

            //legs must extend to jump
            _isCrouching = false;
            _isSliding = false;

            ////////TEMP FIX UNTIL UI BUILT, REMOVE LATER
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

        //rotate player
        transform.Rotate(0, Input.GetAxis("Mouse X") * MouseSensitivityX, 0);

    }

    protected override void Die()
    {
        Audio sound = Instantiate(DeathAudioPrefab);
        sound.transform.position = transform.position;

        //////open menu, show score, restart or quit

        //player is now dead
    }
}
