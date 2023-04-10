using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] public int BaseMaxHealth = 1000;
    protected int _permMaxHealth;
    internal int MaxHealth { get; private protected set; }
    internal int Health { get; private protected set; }

    internal Faction UnitFaction;

    protected Rigidbody _rigidbody;

    [SerializeField] public Transform Eye;
    protected Transform _gunTip;

    [SerializeField] public Weapon[] Weapons;
    protected Weapon _equipedWeapon;
    protected int _equipedWeaponSlot;
    [SerializeField] public int UnloadedAmmo;

    [SerializeField] public Laser LaserPrefab;
    private const float NoHitLaserLength = 500.0f;

    private const float GroundRayLength = 0.1f;

    internal bool IsAlive { get; private protected set; }

    [SerializeField] public float TriggerDelay = 0.5f;
    [SerializeField] public float MaxInaccuracyDegrees = 15.0f;

    [SerializeField] public Audio Hurt1AudioPrefab;
    [SerializeField] public Audio Hurt2AudioPrefab;
    [SerializeField] public Audio Hurt3AudioPrefab;
    [SerializeField] public Audio DeathAudioPrefab;

    protected float _currentSpeed;
    [SerializeField] public float MovementSpeed = 3f;
    [SerializeField] public float SprintSpeed =6f;
    [SerializeField] public float CrouchSpeed = 2f;
    [SerializeField] public float CrouchHeightRatio = 0.5f;
    [SerializeField] public float CrouchSmoothness = 10.0f;
    [SerializeField] public float SlideSpeed = 7.5f;
    [SerializeField] public float SlideTime = 0.5f;
    [SerializeField] public float SlideCooldown = 1.0f; //cools down from slide activation (not slide completion), prevents slide spamming
    protected float _slideTimer;
    [SerializeField] public float JumpVelocity = 5.0f;

    protected float SimulatedLerpFrameRate = 60.0f;

    protected virtual void Start()
    {
        //get components
        _rigidbody = GetComponent<Rigidbody>();
        //_rigidbody.isKinematic = true;
        //initialize speed
        _currentSpeed = MovementSpeed;

        //initialize unit health
        _permMaxHealth = BaseMaxHealth;
        MaxHealth = _permMaxHealth;
        Health = MaxHealth;

        IsAlive = true;

        if (BaseMaxHealth <= 0)
        {
            throw new System.Exception("Base Max Health must be positive!");
        }

        //equip weapon
        _equipedWeaponSlot = 0;
        EquipWeapon();
    }

    protected bool IsGrounded()
    {
        Vector3 origin = transform.position;
        origin.y += GroundRayLength / 2.0f;
        return Physics.Raycast(origin, Vector3.down, GroundRayLength, LayerMask.GetMask("Ground"));
    }

    #region Health System Methods

    //to be called when healing damage
    internal void Heal(int healAmount)
    {
        Health += healAmount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    //to be called when taking damage
    internal void Damage(int damageAmount, Faction damagingFaction)
    {
        //cannot recieve damage from same faction
        if (damagingFaction != UnitFaction)
        {
            Health -= damageAmount;
            CheckLife();

            //hurt audio
            Audio sound;
            int hurtNum = Random.Range(0, 3);
            switch(hurtNum)
            {
                case 0:
                    sound = Instantiate(Hurt1AudioPrefab);
                    sound.transform.position = transform.position;
                    break;
                case 1:
                    sound = Instantiate(Hurt2AudioPrefab);
                    sound.transform.position = transform.position;
                    break;
                case 2:
                    sound = Instantiate(Hurt3AudioPrefab);
                    sound.transform.position = transform.position;
                    break;
            }
                
        }
    }

    //to be called to restore all health
    internal void RestoreHealth()
    {
        Health = MaxHealth;
    }

    //to be called for max health power-ups
    internal void IncMaxHealth(int increaseAmount)
    {
        MaxHealth += increaseAmount;
        
        if (MaxHealth < 0)
        {
            throw new System.Exception("Max Health Overflow!");
        }

        //keep damage amount the same
        Heal(increaseAmount);
    }

    //to be called for max health power-downs
    internal void DecMaxHealth(int decreaseAmount)
    {
        MaxHealth -= decreaseAmount;

        if (MaxHealth < 0)
        {
            MaxHealth = 0;
        }

        //adjust health to new limits
        Heal(0);
        CheckLife();
    }

    //to be called to remove all max health power-ups and power-downs
    internal void RevertMaxHealth()
    {
        MaxHealth = _permMaxHealth;

        if (MaxHealth < 0)
        {
            throw new System.Exception("Max Health Overflow!");
        }

        //adjust health to new limits
        Heal(0);
        CheckLife();
    }

    //to be called for max health upgrades
    internal void IncPermMaxHealth(int increaseAmount)
    {
        _permMaxHealth += increaseAmount;
        MaxHealth += increaseAmount;

        if (MaxHealth < 0)
        {
            throw new System.Exception("Max Health Overflow!");
        }
        if (_permMaxHealth < 0)
        {
            throw new System.Exception("Perm Max Health Overflow!");
        }

        //keep damage amount the same
        Heal(increaseAmount);
    }

    //to be called for max health downgrades
    internal void DecPermMaxHealth(int decreaseAmount)
    {
        _permMaxHealth -= decreaseAmount;
        MaxHealth -= decreaseAmount;

        if (MaxHealth < 0)
        {
            MaxHealth = 0;
        }
        if (_permMaxHealth < 0)
        {
            _permMaxHealth = 0;
        }

        //adjust health to new limits
        Heal(0);
        CheckLife();
    }

    //to be called to remove all max health power-ups, power-downs, upgrades, and downgrades
    internal void RevertPermMaxHealth()
    {
        _permMaxHealth = BaseMaxHealth;
        MaxHealth = BaseMaxHealth;

        if (BaseMaxHealth <= 0)
        {
            throw new System.Exception("Base Max Health must be positive!");
        }

        //adjust health to new limits
        Heal(0);
    }

    //makes isAlive false if health is too low
    private void CheckLife()
    {
        if (Health <= 0)
        {
            Health = 0;
            IsAlive = false;
            Die();
        }
    }

    #endregion

    #region Laser Methods

    internal void FireLaser(int damage, float lazerInaccuracy, Color laserColor)
    {
        Vector3 laserDirection = InaccurateDirection(Eye.forward, MaxInaccuracyDegrees + lazerInaccuracy);

        Ray ray = new Ray(Eye.position, laserDirection);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //hit something
            DrawLaser(_gunTip.position, hit.point, laserColor);

            Unit unitHit = hit.collider.GetComponentInParent<Unit>();
            if (unitHit != null)
            {
                unitHit.Damage(damage, UnitFaction);
            }
        }
        else
        {
            //no hit
            DrawLaser(_gunTip.position, _gunTip.position + laserDirection * NoHitLaserLength, laserColor);
        }
    }

    private void DrawLaser(Vector3 start, Vector3 end, Color color)
    {
        //////get laser from pool
        Laser laser = Instantiate(LaserPrefab) as Laser;
        laser.Init(start, end, color);
    }

    private Vector3 InaccurateDirection(Vector3 direction, float maxInaccuracy)
    {
        float inaccuracyAlpha = maxInaccuracy / 180.0f;
        if (inaccuracyAlpha > 1.0f)
        {
            inaccuracyAlpha = 1.0f;
        }
        else if (inaccuracyAlpha < 0.0f)
        {
            inaccuracyAlpha = 0.0f;
        }

        return ((Random.onUnitSphere * inaccuracyAlpha) + (direction.normalized * (1 - inaccuracyAlpha))).normalized;
    }

    #endregion

    protected void EquipWeapon()
    {
        //hide current weapon
        try
        {
            _equipedWeapon.transform.localScale = Vector3.zero;
        }
        catch
        { 
            //nothing happens if no weapon is currently equiped
        }

        //equip new weapon
        _equipedWeapon = Weapons[_equipedWeaponSlot];
        _gunTip = _equipedWeapon.BarrelEnd;

        //show new weapon
        _equipedWeapon.transform.localScale = Vector3.one;
    }

    protected abstract void Die();
}
