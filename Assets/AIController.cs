using UnityEngine;
using System.Collections;


public enum EnemyState
{
    Patrolling,
    Following,
    Attacking
}


public class AIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Bite = Animator.StringToHash("Bite");

    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private Transform attackPoint;

    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float stopAtDistance = 0.5f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;
<<<<<<< HEAD
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private GameObject attackUI;
    [SerializeField] private float stopDistance = 1.2f;
=======
    [SerializeField] private float attackRange = 1.2f;
>>>>>>> parent of b95cfad (Newest update)

    private UnityEngine.AI.NavMeshAgent _agent;
    private Animator _animator;
    private EnemyState _state = EnemyState.Patrolling;
    private int _currentPatrolIndex;
    private bool _isWaiting;
    private float _timeSincePlayerLost;
    private bool _isBiting;


    private void Start()
    {
        GoToNextPatrolPoint();
    }


    private void Awake()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animator = GetComponent<Animator>();
<<<<<<< HEAD
        _agent.stoppingDistance = 0.5f;
=======
>>>>>>> parent of b95cfad (Newest update)
    }


    private void Update()
    {
        var distanceToPlayer = Vector3.Distance(player.position, attackPoint.position);

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();
                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    _state = EnemyState.Following;
                }

                break;

            case EnemyState.Following:
<<<<<<< HEAD
=======
                FollowPlayer();
>>>>>>> parent of b95cfad (Newest update)
                if (distanceToPlayer <= attackRange)
                {
                    Debug.Log("SWITCHING TO ATTACK");
                    _state = EnemyState.Attacking;
                    StartAttack();
                    break;
                }
                if (!CanSeePlayer())
                {
                    _timeSincePlayerLost += Time.deltaTime;
                    if (_timeSincePlayerLost >= losePlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                } else
                {
                    _timeSincePlayerLost = 0f;
                }

                break;

            case EnemyState.Attacking:
                Attack();
<<<<<<< HEAD
                //if (!_isBiting && distanceToPlayer > attackRange)
                //{
                    //_state = EnemyState.Following;
                    //_agent.isStopped = false;

                //} 

                break;
        }
        if (attackUI != null)
        {
            attackUI.SetActive(_state == EnemyState.Attacking);
        }
        Debug.Log("Current State: " + _state);
        UpdateAnimations();
        
=======
                if (!_isBiting && distanceToPlayer > attackRange)
                {
                    _state = EnemyState.Following;
                    _agent.isStopped = false;
                } 

                break;
        }
>>>>>>> parent of b95cfad (Newest update)

        UpdateAnimations();
    }

    private void FollowPlayer()
    {
        _agent.SetDestination(player.position);
    }

    private void Attack()
    {
        _agent.isStopped = true;

        var direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        if (!_isBiting)
        {
            _isBiting = true;
            _animator.SetTrigger(Bite);
        }
    }

    public void OnBiteAnimationEnd()
    {
        _isBiting = false;
    }

    private void Patrol()
    {
        if (_isWaiting) return;
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

     private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        _agent.isStopped = false;
        GoToNextPatrolPoint();
        _isWaiting = false;
    }

    private void StartAttack()
    {
        Debug.Log("START ATTACK CALLED");
        _agent.isStopped = true;
        _isBiting = true;
        _animator.SetTrigger(Bite);
    }


    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void UpdateAnimations()
    {
        bool isMoving =
            _agent.hasPath &&
            !_agent.pathPending &&
            _agent.remainingDistance > _agent.stoppingDistance &&
            _agent.velocity.magnitude > 0.1f;

        _animator.SetBool(IsWalking, isMoving);
    }

    private bool CanSeePlayer()
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    private bool IsFacingPlayer()
    {
        var dirToPlayer = (player.position - transform.position).normalized;
        var angle = Vector3.Angle(transform.forward,dirToPlayer);
        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        var dirToPlayer = player.position - transform.position;
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        var closestIndex = 0;
        var closestDistance = float.MaxValue;

        for (var i = 0; i < patrolPoints.Length; i++)
        {
            var distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);

    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
