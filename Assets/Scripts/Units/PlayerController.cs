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

    [SerializeField] public float CrouchCameraSpeed = 5.0f;

    [SerializeField] public Audio WeaponChangeAudioPrefab;

    protected bool _isSprinting = false;
    protected bool _isCrouching = false;
    protected bool _isSliding = false;
    protected bool _isSlideCooling = false;
    private bool _isJumping = false;
    private bool _cursorIsLocked=true;

    [SerializeField] public float BodyHeight = 1.75f;

    [SerializeField] public float PowerDamageMultiplier = 2.0f;
    [SerializeField] public float PowerSpeedMultiplier = 1.5f;
    private float _powerDamageTimer;
    private float _powerSpeedTimer;
    private float _powerInvincibilityTimer;
    private float _powerUnlimitedAmmoTimer;

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

        //equip weapon
        _equipedWeaponSlot = 0;
        EquipWeapon();
    }

    private void Update()
    {
        //Update powerup timers
        _powerDamageTimer -= Time.deltaTime;
        _powerSpeedTimer -= Time.deltaTime;
        _powerInvincibilityTimer -= Time.deltaTime;
        _powerUnlimitedAmmoTimer -= Time.deltaTime;

        //rotate camera
        _cameraPivot.Rotate(-Input.GetAxis("Mouse Y") * MouseSensitivityY, 0, 0);
       //lock mouse
        InternalLockUpdate();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.Instance.IsPlayGame)
            {
                UIManager.Instance.Pause();
            }
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

        //switch weapon forward
        if (Input.GetKeyDown(KeyCode.E))
        {
            _equipedWeaponSlot++;
            if (_equipedWeaponSlot >= Weapons.Length)
            {
                _equipedWeaponSlot = 0;
            }

            EquipWeapon();

            UIManager.Instance.UpdateGun();

            Audio sound = Instantiate(WeaponChangeAudioPrefab);
            sound.transform.position = transform.position;
        }

        //switch weapon backward
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _equipedWeaponSlot--;
            if (_equipedWeaponSlot < 0)
            {
                _equipedWeaponSlot = Weapons.Length - 1;
            }

            EquipWeapon();

            UIManager.Instance.UpdateGun();

            Audio sound = Instantiate(WeaponChangeAudioPrefab);
            sound.transform.position = transform.position;
        }

        //switch to weapon 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            try
            {
                _equipedWeaponSlot = 0;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player has no weapons
                //do nothing
            }
        }

        //switch to weapon 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            try
            {
                _equipedWeaponSlot = 1;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 2 weapons
                //do nothing
            }
        }

        //switch to weapon 3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            try
            {
                _equipedWeaponSlot = 2;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 3 weapons
                //do nothing
            }
        }

        //switch to weapon 4
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            try
            {
                _equipedWeaponSlot = 3;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 4 weapons
                //do nothing
            }
        }

        //switch to weapon 5
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            try
            {
                _equipedWeaponSlot = 4;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 5 weapons
                //do nothing
            }
        }

        //switch to weapon 6
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            try
            {
                _equipedWeaponSlot = 5;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 6 weapons
                //do nothing
            }
        }

        //switch to weapon 7
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            try
            {
                _equipedWeaponSlot = 6;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 7 weapons
                //do nothing
            }
        }

        //switch to weapon 8
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            try
            {
                _equipedWeaponSlot = 7;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 8 weapons
                //do nothing
            }
        }

        //switch to weapon 9
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            try
            {
                _equipedWeaponSlot = 8;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 9 weapons
                //do nothing
            }
        }

        //switch to weapon 10
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            try
            {
                _equipedWeaponSlot = 9;

                EquipWeapon();

                UIManager.Instance.UpdateGun();

                Audio sound = Instantiate(WeaponChangeAudioPrefab);
                sound.transform.position = transform.position;
            }
            catch
            {
                //player doesn't have 10 weapons
                //do nothing
            }
        }

        //reload weapon
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnloadedAmmo = EquipedWeapon.Reload(UnloadedAmmo);

            UIManager.Instance.UpdateGun();
            UIManager.Instance.UpdateAmmo();
        }

        //fire weapon
        if (Input.GetMouseButton(0) && EquipedWeapon.IsAutomatic)
        {
            EquipedWeapon.Fire(this, false);

            UIManager.Instance.UpdateGun();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            EquipedWeapon.Fire(this, false);

            UIManager.Instance.UpdateGun();
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
/// <summary>
/// lock mouse
/// </summary>
    protected override void Die()
    {
        //do once
        if (IsAlive)
        {
            IsAlive = false;

            Audio sound = Instantiate(DeathAudioPrefab);
            sound.transform.position = transform.position;

            //open menu, show score, restart or quit
            UIManager.Instance.ShowEndGameMenu();

            //player is now dead
        }
    }
    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _cursorIsLocked = true;
        }

        if (_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #region Health System

    internal override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        UIManager.Instance.UpdateHealth();
    }

    internal override void Damage(int damageAmount, Faction damagingFaction)
    {
        base.Damage(damageAmount, damagingFaction);
        UIManager.Instance.UpdateHealth();
    }

    internal override void RestoreHealth()
    {
        base.RestoreHealth();
        UIManager.Instance.UpdateHealth();
    }

    internal override void IncMaxHealth(int increaseAmount)
    {
        base.IncMaxHealth(increaseAmount);
        UIManager.Instance.UpdateHealth();
    }

    internal override void DecMaxHealth(int decreaseAmount)
    {
        base.DecMaxHealth(decreaseAmount);
        UIManager.Instance.UpdateHealth();
    }

    internal override void RevertMaxHealth()
    {
        base.RevertMaxHealth();
        UIManager.Instance.UpdateHealth();
    }

    internal override void IncPermMaxHealth(int increaseAmount)
    {
        base.IncPermMaxHealth(increaseAmount);
        UIManager.Instance.UpdateHealth();
    }

    internal override void DecPermMaxHealth(int decreaseAmount)
    {
        base.DecPermMaxHealth(decreaseAmount);
        UIManager.Instance.UpdateHealth();
    }

    internal override void RevertPermMaxHealth()
    {
        base.RevertPermMaxHealth();
        UIManager.Instance.UpdateHealth();
    }

    #endregion

    internal void PowerupDamage(float time)
    {
        
    }

    internal void PowerupSpeed(float time)
    {
        
    }

    internal void PowerupInvincibility(float time)
    { 
        
    }

    internal void PowerupUnlimitedAmmo(float time)
    {
        
    }
}
