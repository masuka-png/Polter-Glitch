using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Follow,
    Attack
}

public class EnemyAI : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Bite = Animator.StringToHash("Bite");

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float losePlayerTime = 3f;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip clip;

    private NavMeshAgent agent;
    private Animator anim;

    private EnemyState state = EnemyState.Patrol;
    private int patrolIndex;
    private float lastSeenTimer;
    private float lastAttackTime;
    private bool isWaiting;
    private bool isAttacking;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SetNextPatrolPoint();
    }

    private void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        bool canSee = CanSeePlayer();

        switch (state)
        {
            case EnemyState.Patrol:
                HandlePatrol();

                if (dist <= detectionRange && canSee)
                {
                    source.PlayOneShot(clip);
                    state = EnemyState.Follow;
                }
                break;

            case EnemyState.Follow:
                agent.SetDestination(player.position);

                if (dist <= attackRange && canSee && Time.time >= lastAttackTime + attackCooldown)
                {
                    StartAttack();
                    state = EnemyState.Attack;
                }

                if (!canSee)
                {
                    lastSeenTimer += Time.deltaTime;
                    if (lastSeenTimer >= losePlayerTime)
                    {
                        state = EnemyState.Patrol;
                        GoToClosestPatrol();
                    }
                }
                else lastSeenTimer = 0f;

                break;

            case EnemyState.Attack:
                FacePlayer();

                if (!isAttacking && (dist > attackRange || !canSee))
                {
                    state = EnemyState.Patrol;
                    agent.isStopped = false;
                    GoToClosestPatrol();
                }
                break;
        }

        anim.SetBool(IsWalking, agent.velocity.sqrMagnitude > 0.01f);
    }

    // -------- PATROL --------
    private void HandlePatrol()
    {
        if (isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
            StartCoroutine(PatrolWait());
    }

    private IEnumerator PatrolWait()
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(patrolWaitTime);

        agent.isStopped = false;
        SetNextPatrolPoint();
        isWaiting = false;
    }

    private void SetNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[patrolIndex].position);
        patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
    }

    private void GoToClosestPatrol()
    {
        int closest = 0;
        float best = Mathf.Infinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (d < best)
            {
                best = d;
                closest = i;
            }
        }

        patrolIndex = closest;
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    // -------- ATTACK --------
    private void StartAttack()
    {
        lastAttackTime = Time.time;
        agent.isStopped = true;
        isAttacking = true;
        anim.SetTrigger(Bite);
    }

    private void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    // Animation Event
    public void OnBiteAnimationEnd()
    {
        isAttacking = false;
    }

    // -------- VISION --------
    private bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;

        if (Vector3.Angle(transform.forward, dir) > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, detectionRange))
            return hit.transform == player;

        return false;
    }
}