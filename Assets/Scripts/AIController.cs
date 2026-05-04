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
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Bite = Animator.StringToHash("Bite");

    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;
    [SerializeField] private float attackRange = 1.2f;

    [Header("UI")]
    [SerializeField] private GameObject attackUI;

    private UnityEngine.AI.NavMeshAgent _agent;
    private Animator _animator;
    private EnemyState _state = EnemyState.Patrolling;

    private int _currentPatrolIndex;
    private bool _isWaiting;
    private float _timeSincePlayerLost;
    private bool _isBiting;

    public AudioSource source;
    public AudioClip clip;

    private PlayerUIController _playerUI;

    [Header("Player Reference")]
    public EasyPeasyFirstPersonController.FirstPersonController controller;

    private void Awake()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _playerUI = player.GetComponent<PlayerUIController>();
    }

    private void Start()
    {
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, attackPoint.position);

        switch (_state)
        {
            case EnemyState.Patrolling:
                Patrol();

                if (distanceToPlayer <= detectionRange && CanSeePlayer())
                {
                    source.PlayOneShot(clip);
                    _state = EnemyState.Following;
                }
                break;

            case EnemyState.Following:
                FollowPlayer();

                if (distanceToPlayer <= attackRange)
                {
                    _state = EnemyState.Attacking;
                    StartAttack();
                }

                if (!CanSeePlayer())
                {
                    _timeSincePlayerLost += Time.deltaTime;

                    if (_timeSincePlayerLost >= losePlayerTime)
                    {
                        _state = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    _timeSincePlayerLost = 0f;
                }
                break;

            case EnemyState.Attacking:
                Attack();

                if (!_isBiting && distanceToPlayer > attackRange)
                {
                    StopAttack();
                    _state = EnemyState.Following;
                }
                break;
        }

        UpdateAnimations();
    }

    private void FollowPlayer()
    {
        _agent.isStopped = false;
        _agent.SetDestination(player.position);
    }

    private void StartAttack()
    {
        Debug.Log("START ATTACK");

        _agent.isStopped = true;
        _isBiting = true;

        // Show UI
        if (_playerUI != null)
            _playerUI.ShowAttackUI();

        if (attackUI != null)
            attackUI.SetActive(true);

        // 🔥 IMPORTANT: Enable UI mode
        if (controller != null)
            controller.EnterUIMode();

        _animator.SetTrigger(Bite);
    }

    private void StopAttack()
    {
        Debug.Log("STOP ATTACK");

        // Hide UI
        if (_playerUI != null)
            _playerUI.HideAttackUI();

        if (attackUI != null)
            attackUI.SetActive(false);

        // 🔥 IMPORTANT: Return control to player
        if (controller != null)
            controller.ExitUIMode();

        _agent.isStopped = false;
        _isBiting = false;
    }

    private void Attack()
    {
        _agent.isStopped = true;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

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

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
    }

    private void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        _currentPatrolIndex = closestIndex;
        _agent.SetDestination(patrolPoints[_currentPatrolIndex].position);
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
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        return angle <= viewAngle / 2f;
    }

    private bool HasClearPathToPlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;

        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, dirToPlayer.magnitude))
        {
            return hit.transform == player;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}