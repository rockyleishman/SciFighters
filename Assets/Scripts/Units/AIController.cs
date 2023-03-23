using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIController : Unit
{
    private NavMeshAgent _agent;

    private AIState _currentState;

    [SerializeField] public Transform Eye;
    [SerializeField] public Transform GunTip;
    [SerializeField] public float ViewAngle = 75.0f;
    [SerializeField] public float AimAngle = 3.0f;
    [SerializeField] public float DetectionDistance = 25.0f;
    [SerializeField] public float ChaseDistance = 50.0f;
    [SerializeField] public float ChaseTime = 30.0f;
    private float _chaseTimer;

    [SerializeField] public float PatrolPointReachedRadius = 5.0f;
    [SerializeField] [Range(0.0f, 60.0f)] public float PatrolPointSwitchAverageSeconds = 10.0f;

    [SerializeField] public float TriggerDelay = 0.5f;
    [SerializeField] public float MaxInaccuracyDegrees = 5.0f;
    [SerializeField] public float PrimaryDamage = 10.0f;
    [SerializeField] public float SecondaryDamage = 50.0f;
    [SerializeField] public float MeleeDamage = 10.0f;

    private PatrolPoint _currentPatrolPoint;
    private Unit _currentEnemy;

    protected override void Start()
    {
        base.Start();

        _agent = GetComponent<NavMeshAgent>();

        SetState(AIState.Idle);

        _chaseTimer = 0.0f;
    }

    void Update()
    {
        
    }

    private void SetState(AIState newState)
    {
        _currentState = newState;

        StopAllCoroutines();

        switch (_currentState)
        {
            case AIState.Roam:
                StartCoroutine(OnRoam());
                break;
            case AIState.Chase:
                StartCoroutine(OnChase());
                break;
            case AIState.Attack:
                StartCoroutine(OnAttack());
                break;
            case AIState.Idle:
            default:
                StartCoroutine(OnIdle());
                break;
        }
    }

    private IEnumerator OnIdle()
    {
        while (_currentPatrolPoint == null)
        {
            LookForNewPatrolPoint();
            yield return null;
        }
        SetState(AIState.Roam);
    }

    private IEnumerator OnRoam()
    {
        _agent.SetDestination(_currentPatrolPoint.transform.position);

        //while not near patrol point
        while (Vector3.Distance(transform.position, _currentPatrolPoint.transform.position) > PatrolPointReachedRadius)
        {
            DetectEnemies();
            ResetPatrolPointChance();
            yield return null;
        }

        _currentPatrolPoint = null;
        SetState(AIState.Idle);
    }

    private IEnumerator OnChase()
    {
        //goto current enemy
        _agent.ResetPath();

        //reset chase timer
        _chaseTimer = 0.0f;

        while (_currentEnemy.IsAlive && !CanSeeTarget(_currentEnemy.transform, GunTip, AimAngle) && Vector3.Distance(Eye.position, _currentEnemy.transform.position) <= ChaseDistance && _chaseTimer < ChaseTime)
        {
            _agent.SetDestination(_currentEnemy.transform.position);
            _chaseTimer += Time.deltaTime;
            yield return null;
        }

        if (CanSeeTarget(_currentEnemy.transform, GunTip, AimAngle))
        {
            SetState(AIState.Attack);
        }
        else
        {
            _currentEnemy = null;
            SetState(AIState.Idle);
        }
    }

    private IEnumerator OnAttack()
    {
        //attack current enemy
        while (_currentEnemy.IsAlive && CanSeeTarget(_currentEnemy.transform, GunTip, AimAngle))
        {
            AttackEnemy();
            yield return null;
        }

        if (_currentEnemy.IsAlive)
        {
            SetState(AIState.Chase);
        }
        else
        {
            SetState(AIState.Idle);
        }
    }

    private void LookForNewPatrolPoint()
    {
        _currentPatrolPoint = GameManager.Instance.LevelPatrolPoints[Random.Range(0, GameManager.Instance.LevelPatrolPoints.Length)];
    }

    protected bool CanSeeTarget(Transform target, Transform eye, float viewAngle)
    {
        RaycastHit hit;

        //cannot see if not within view angle
        if (Vector3.Angle(transform.forward, target.position - eye.position) > viewAngle)
        {
            return false;
        }

        //check if direct line of sight
        if (Physics.Raycast(new Ray(eye.position, target.position - eye.position), out hit, Mathf.Infinity, ~LayerMask.GetMask("null")))
        {
            //cannot see if something is in the way
            if (hit.transform != target)
            {
                return false;
            }
        }

        return true;
    }

    protected abstract void AttackEnemy();
 
    private void DetectEnemies()
    {
        //detect enemies
        Collider[] surroundingColliders = Physics.OverlapSphere(this.transform.position, DetectionDistance);
        foreach (Collider collider in surroundingColliders)
        {
            Unit unit = collider.GetComponentInParent<Unit>();

            if (unit != null && unit != this && unit.UnitFaction != UnitFaction && unit.IsAlive && CanSeeTarget(unit.transform, Eye, ViewAngle))
            {
                _currentEnemy = unit;
                SetState(AIState.Chase);
                break; //lock onto first enemy detected, ignore the rest
            }
        }
    }

    //enhance roam simulation
    private void ResetPatrolPointChance()
    {
        if (Random.Range(0.0f, 1.0f) < 1.0f / PatrolPointSwitchAverageSeconds * Time.deltaTime)
        {
            _currentPatrolPoint = null;
            SetState(AIState.Idle);
        }
    }
}
