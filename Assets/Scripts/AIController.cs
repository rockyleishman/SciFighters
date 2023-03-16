using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : Unit
{
    private NavMeshAgent _agent;

    private AIState _currentState;

    public Transform Eye;
    public float ViewAngle = 75.0f;
    public float DetectionDistance = 50.0f;
    public float ChaseDistance = 100.0f;

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
                //////
                break;
            case AIState.Chase:
                //////
                break;
            case AIState.Attack:
                //////
                break;
            case AIState.Idle:
            default:
                //////
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
        //////while not near patrol point
        ///detect enemies();
        ///chance to reset patrol point();
        yield return null;

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
}
