using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Unit
{
    private Camera _playerCamera; //for aiming
    private Transform _cameraPivot; //for moving view angle
    [SerializeField] public float MouseSensitivityX = 1.0f;
    [SerializeField] public float MouseSensitivityY = 1.0f;

    private bool _isJumping = false;

    protected override void Start()
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
        if (Input.GetKeyDown(KeyCode.LeftControl) && !_isSlideCooling && (Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f))
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
            _currentSpeed = SprintSpeed;
            _isSprinting = true;
            _isCrouching = false;
            _isSliding = false;
        }
        else
        {
            _currentSpeed = MovementSpeed;
            _isSprinting = false;
            _isCrouching = false;
            _isSliding = false;
        }

        //change collider height for crouching
        if (_isCrouching)
        {
            UpdateHitBoxHeight(CrouchHeightRatio);
        }
        else
        {
            UpdateHitBoxHeight(1.0f);
        }

        //get movement input
        Vector3 movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * _currentSpeed;

        //get jump input
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            _isJumping = true;
            movementInput.y = JumpVelocity;
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
