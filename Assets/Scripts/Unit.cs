using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public int FullHealth = 100;
    internal int Health;

    internal Faction UnitFaction;

    protected Rigidbody _rigidbody;

    private const float GroundRayLength = 0.1f;

    protected bool isAlive = true;

    protected float unitHieght;

    [SerializeField] public float MovementSpeed = 5.0f;
    [SerializeField] public float JumpVelocity = 5.0f;

    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        unitHieght = GetComponent<CapsuleCollider>().height;
    }

    protected bool IsGrounded()
    {
        Vector3 origin = transform.position;
        origin.y += (GroundRayLength - unitHieght) / 2.0f;
        return Physics.Raycast(origin, Vector3.down, GroundRayLength, LayerMask.GetMask("Ground"));
    }

    
}
