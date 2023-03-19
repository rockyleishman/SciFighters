using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : Unit
{
    private NavMeshAgent _agent;

    private AIState _currentState;

    [SerializeField] public Transform Eye;
    [SerializeField] public float ViewAngle = 75.0f;
    [SerializeField] public float DetectionDistance = 50.0f;
    [SerializeField] public float ChaseDistance = 100.0f;

    [SerializeField] public float PatrolPointReachedRadius = 5.0f;
    [SerializeField] [Range(0.0f, 1.0f)] public float PatrolPointSwitchAverageSeconds = 15.0f;

    [SerializeField] public float TriggerDelay = 0.5f;
    [SerializeField] public float MaxInaccuracyDegrees = 5.0f;
    [SerializeField] public float PrimaryDamage = 10.0f;
    [SerializeField] public float SecondaryDamage = 50.0f;
    [SerializeField] public float MeleeDamage = 10.0f;

    private PatrolPoint _currentPatrolPoint;
    private Unit _currentEnemy;

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

        while (_currentEnemy.IsAlive && !CanSeeTarget(_currentEnemy.transform) && Vector3.Distance(Eye.position, _currentEnemy.transform.position) <= ChaseDistance)
        {
            _agent.SetDestination(_currentEnemy.transform.position);
            yield return null;
        }

        if (CanSeeTarget(_currentEnemy.transform))
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
        //////attack current enemy
        yield return null;
    }

    private void LookForNewPatrolPoint()
    {
        _currentPatrolPoint = GameManager.Instance.LevelPatrolPoints[Random.Range(0, GameManager.Instance.LevelPatrolPoints.Length)];
    }

    protected bool CanSeeTarget(Transform target)
    {
        RaycastHit hit;

        //cannot see if not within view angle
        if (Vector3.Angle(transform.forward, target.position - Eye.position) > ViewAngle)
        {
            return false;
        }

        //check if direct line of sight
        if (Physics.Raycast(new Ray(Eye.position, target.position - Eye.position), out hit, Mathf.Infinity, ~LayerMask.GetMask("null")))
        {
            //cannot see if something is in the way
            if (hit.transform != target)
            {
                return false;
            }
        }

        return true;
    }

    private void DetectEnemies()
    { 
        //////detect enemies
    }

    //enhance roam simulation
    private void ResetPatrolPointChance()
    {
        if (Random.Range(0.0f, 1.0f) < 1.0f / PatrolPointSwitchAverageSeconds * Time.deltaTime)
        {
            LookForNewPatrolPoint();
        }
    }
}
