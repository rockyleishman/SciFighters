using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIController : Unit
{
    private NavMeshAgent _agent;

    private AIState _currentState;
    [SerializeField] public Animator ani;     // define the animator
    [SerializeField] public float ViewAngle = 75.0f;
    [SerializeField] public float AimAngle = 5.0f;
    [SerializeField] public float DetectionDistance = 25.0f;
    [SerializeField] public float ChaseDistance = 50.0f;
    [SerializeField] public float ChaseTime = 30.0f;
    private float _chaseTimer;

    [SerializeField] public float PatrolPointReachedRadius = 5.0f;
    [SerializeField] [Range(0.0f, 60.0f)] public float PatrolPointSwitchAverageSeconds = 10.0f;

    [SerializeField] public float PrimaryDamage = 10.0f;
    [SerializeField] public float SecondaryDamage = 50.0f;
    [SerializeField] public float MeleeDamage = 10.0f;

    private PatrolPoint _currentPatrolPoint;
    private Unit _currentEnemy;

    protected override void Start()
    {
        base.Start();
        ani = this.GetComponentInChildren<Animator>();  //get the animator in child
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MovementSpeed;

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
            
            //set Idle animation
            ani.SetFloat("Horizontal", 0, 0.05f, Time.deltaTime);
            ani.SetFloat("Vertical", 0, 0.05f, Time.deltaTime);
            
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
            
            //set Roam animation
            ani.SetFloat("Horizontal", 0, 0.05f, Time.deltaTime);   
            ani.SetFloat("Vertical", 1, 0.05f, Time.deltaTime);
            ani.SetBool("isRunning", false);
            ani.SetBool("FireBool",false);
            yield return null;
        }

        _currentPatrolPoint = null;
        SetState(AIState.Idle);
    }

    private IEnumerator OnChase()
    {
        //stop current path
        _agent.ResetPath();

        //reset chase timer
        _chaseTimer = 0.0f;

        while (_currentEnemy.IsAlive && !CanSeeTarget(_currentEnemy.transform, Eye, ViewAngle) && Vector3.Distance(Eye.position, _currentEnemy.transform.position) <= ChaseDistance && _chaseTimer < ChaseTime)
        {
            _agent.SetDestination(_currentEnemy.transform.position);
            _chaseTimer += Time.deltaTime;
            
            //set Chase animation
            ani.SetBool("isRunning", true);
            ani.SetBool("FireBool",false);
            yield return null;
        }

        if (CanSeeTarget(_currentEnemy.transform, Eye, ViewAngle))
        {
            _agent.ResetPath();
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
        while (_currentEnemy.IsAlive && CanSeeTarget(_currentEnemy.transform, Eye, ViewAngle))
        {
            //turn
            transform.LookAt(_currentEnemy.transform);
            transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, 0.0f);

            //aim
            //Eye.transform.LookAt(_currentEnemy.transform);
            //Eye.transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, transform.localEulerAngles.z);

            //attack
            AttackEnemy();
            
            //set Attack animation
            ani.SetBool("isRunning", false);
            ani.SetBool("FireBool",true);
            
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
            ////////
            Debug.Log("out of view angle");

            return false;
        }

        //check if direct line of sight
        if (Physics.Raycast(new Ray(eye.position, target.position - eye.position), out hit, Mathf.Infinity, ~LayerMask.GetMask("null")))
        {
            //cannot see if something is in the way
            if (hit.transform != target)
            {
                Debug.Log("something in the way");

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

    protected override void Die()
    {
        //set death animation
        ani.SetTrigger(("DeadTrigger"));
        
        Audio sound = Instantiate(DeathAudioPrefab);
        sound.transform.position = transform.position;
        
        this.enabled = false;
        //delay 2s to destory the AI
        Destroy(this.gameObject, 2f);
    }
}
