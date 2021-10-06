using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{

    public Transform player;
    public NavMeshAgent agent;
    public GameObject projectile;
    Animator animator;

    EnemyGun enemyGun;

    public LayerMask whatIsGround, whatIsPlayer;

    public float damage = 100f;

    //Patroling
    public Transform[] patrolPoints;
    private int currentPatrolPoint;

    //Attacking
    public float timeBetweenAttacks = 0.425f;
    bool alreadyAttacked;

    //States
    public float attackRange;
    public bool playerInAttackRange;

    public Slider healthBar;
    public Image Bomb;

    private Target target;

    private void Awake()
    {
        currentPatrolPoint = 0;
        player = GameObject.Find("Ken").transform;
        agent = GetComponent<NavMeshAgent>();
        Bomb.gameObject.SetActive(false);
    }

    void Start()
    {
        target = GetComponent<Target>();
        healthBar.maxValue = target.health;
        enemyGun = GetComponent<EnemyGun>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(target.getInRange() == true)
        {
            Bomb.gameObject.SetActive(true);
        }
        else
        {
            Bomb.gameObject.SetActive(false);
        }
        healthBar.value = target.health;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInAttackRange) Patroling();
        if (playerInAttackRange && !animator.GetBool("isDead")) AttackPlayer();
    }

    private void Patroling()
    {
        transform.LookAt(patrolPoints[currentPatrolPoint].position);
        agent.SetDestination(patrolPoints[currentPatrolPoint].position);

        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position);

        if (distanceToPatrolPoint < 1f)
            changePatrolPoint();
    }
    
    private void changePatrolPoint()
    {
        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;

        transform.LookAt(patrolPoints[currentPatrolPoint].position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        animator.SetBool("isFiring", true);

        if (!alreadyAttacked)
        {
            enemyGun.ShootPlayer(player.transform);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("isFiring", false);
    }
}
