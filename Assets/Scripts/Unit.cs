using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public int BaseMaxHealth = 100;
    protected int _permMaxHealth;
    internal int MaxHealth { get; private protected set; }
    internal int Health { get; private protected set; }

    internal Faction UnitFaction;

    protected Rigidbody _rigidbody;

    private const float GroundRayLength = 0.1f;
    protected float unitHieght;

    protected bool isAlive = true;

    [SerializeField] public float MovementSpeed = 5.0f;
    [SerializeField] public float SprintSpeed = 7.5f;
    [SerializeField] public float JumpVelocity = 5.0f;

    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        unitHieght = GetComponent<CapsuleCollider>().height;

        //initialize unit health
        _permMaxHealth = BaseMaxHealth;
        MaxHealth = _permMaxHealth;
        Health = MaxHealth;

        if (BaseMaxHealth <= 0)
        {
            throw new System.Exception("Base Max Health must be positive!");
        }
    }

    protected bool IsGrounded()
    {
        Vector3 origin = transform.position;
        origin.y += (GroundRayLength - unitHieght) / 2.0f;
        return Physics.Raycast(origin, Vector3.down, GroundRayLength, LayerMask.GetMask("Ground"));
    }

    #region Health System Methods

    //to be called when healing damage
    public void Heal(int healAmount)
    {
        Health += healAmount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    //to be called when taking damage
    public void Damage(int damageAmount)
    {
        Health -= damageAmount;

        if (Health <= 0)
        {
            Health = 0;
            isAlive = false;
        }
    }

    //to be called to restore all health
    public void RestoreHealth()
    {
        Health = MaxHealth;
    }

    //to be called for max health power-ups
    public void IncMaxHealth(int increaseAmount)
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
    public void DecMaxHealth(int decreaseAmount)
    {
        MaxHealth -= decreaseAmount;

        if (MaxHealth < 0)
        {
            MaxHealth = 0;
        }

        //adjust health to new limits
        Heal(0);
        Damage(0);
    }

    //to be called to remove all max health power-ups and power-downs
    public void RevertMaxHealth()
    {
        MaxHealth = _permMaxHealth;

        if (MaxHealth < 0)
        {
            throw new System.Exception("Max Health Overflow!");
        }

        //adjust health to new limits
        Heal(0);
        Damage(0);
    }

    //to be called for max health upgrades
    public void IncPermMaxHealth(int increaseAmount)
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
    public void DecPermMaxHealth(int decreaseAmount)
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
        Damage(0);
    }

    //to be called to remove all max health power-ups, power-downs, upgrades, and downgrades
    public void RevertPermMaxHealth()
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

    #endregion
}
