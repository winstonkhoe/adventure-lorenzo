using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{

    public bool inPosition;

    public Transform player;
    public Transform diePosition;
    public NavMeshAgent agent;

    public LayerMask whatIsGround, whatIsPlayer;

    public int chancesOfDrop;

    

    

    private void Awake()
    {
        patrolPoints = new List<Transform>();
        currentPatrolPoint = 0;
        player = GameObject.Find("Ken").transform;
        agent = GetComponent<NavMeshAgent>();
        Bomb.gameObject.SetActive(false);
    }

    void Start()
    {
        initPlayer();
    }

    void Update()
    {
        if(inRange == true)
        {
            Bomb.gameObject.SetActive(true);
        }
        else
        {
            Bomb.gameObject.SetActive(false);
        }
        healthBar.value = health;
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);
        if (inPosition && playerInChaseRange && !playerInAttackRange) ChasePlayer();
        if (inPosition && !playerInAttackRange) Patroling();
        if (playerInAttackRange && !animator.GetBool("isDead")) AttackPlayer();
    }

    #region Enemy
    Animator animator;

    EnemyGun enemyGun;

    public Slider healthBar;
    public Image Bomb;

    public GameObject coreItem;
    private bool droppedCoreItem = false;

    public int health = 50;
    private bool inRange = false;

    public int spawnDelay;

    void initPlayer()
    {
        enemyGun = GetComponent<EnemyGun>();
        animator = GetComponent<Animator>();
        healthBar.maxValue = health;
        inPosition = false;
    }

    public void setInRange(bool value)
    {
        inRange = value;
    }

    public bool getInRange()
    {
        return inRange;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public bool willDropItem()
    {
        if (Random.Range(1, 10) <= chancesOfDrop / 10)
            return true;

        return false;
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        //controller.Move(new Vector3(0, -5f).normalized * Time.deltaTime);
        Vector3 destinationDrop = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
        if (gameObject.tag.Equals("Enemy") && droppedCoreItem == false)
        {
            droppedCoreItem = true;
            if(willDropItem())
            {
                DropItem dropItem = FindObjectOfType<DropItem>();
                Instantiate(dropItem.randomItemDrop(), transform.position, Quaternion.identity);
            }
            Instantiate(coreItem, transform.position, Quaternion.identity);
        }
        GenerateEnemy ge = FindObjectOfType<GenerateEnemy>();
        Debug.Log("Enemy at patrolIndex " + patrolIndex + " is Dead");
        ge.cleanPatroliExist(gameObject.name, patrolIndex, spawnDelay);
        //Vector3.Lerp(transform.position, diePosition.position, 2);
        //transform.Translate(destinationDrop * Time.deltaTime, Space.World);
        Destroy(gameObject, 4f);
    }
    #endregion

    //Patroling
    public int patrolIndex; //BigPoint before smaller points

    public List<Transform> patrolPoints;
    private int currentPatrolPoint;
    private void Patroling()
    {
        //Debug.Log("Patroling");
        transform.LookAt(patrolPoints[currentPatrolPoint].position);
        agent.SetDestination(patrolPoints[currentPatrolPoint].position);

        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position);

        if (distanceToPatrolPoint < 1f)
            changePatrolPoint();
    }
    
    private void changePatrolPoint()
    {
        currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Count;

        transform.LookAt(patrolPoints[currentPatrolPoint].position);
    }


    #region Enemy Attacking

    bool alreadyAttacked;

    //States
    public float attackRange;
    public float chaseRange;

    public bool playerInAttackRange;
    public bool playerInChaseRange;

    public float timeBetweenAttacks = 0.425f;
    public float damage = 100f;
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        //Debug.Log(player.position.y);
        //Debug.Log(player.GetComponent<Collider>().bounds.size.y);
        float playerHeight = player.GetComponent<Collider>().bounds.size.y;
        Vector3 newLook = new Vector3(player.position.x, player.position.y + playerHeight/2, player.position.z);
        //Debug.Log(newLook);
        transform.LookAt(newLook);

        animator.SetBool("isFiring", true);

        if (!alreadyAttacked)
        {
            Debug.Log(player.name);
            Debug.Log(player.position);
            enemyGun.ShootPlayer(player);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ChasePlayer()
    {
        float playerHeight = player.GetComponent<Collider>().bounds.size.y;
        Vector3 newLook = new Vector3(player.position.x, player.position.y + playerHeight / 2, player.position.z);
        transform.LookAt(newLook);
        agent.SetDestination(player.position);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("isFiring", false);
    }

    #endregion
}
