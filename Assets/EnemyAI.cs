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
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

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
        player = GameObject.Find("Ken").transform;
        agent = GetComponent<NavMeshAgent>();
        Bomb.gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Target>();
        healthBar.maxValue = target.health;
        enemyGun = GetComponent<EnemyGun>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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
        if (playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        //Debug.Log("Patroling");
        if (!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    
    private void SearchWalkPoint()
    {
        //float randomZ = Random.Range(-walkPointRange, walkPointRange);
        //float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = 20;
        float randomX = 0;

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        animator.SetBool("isFiring", true);
        RaycastHit hit;

        if (!alreadyAttacked)
        {
            enemyGun.ShootPlayer(player.transform);
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            //if (Physics.Raycast(rb.position, rb.transform.forward, out hit, 5f))
            //{

            //    Target target = hit.transform.GetComponent<Target>();
            //    Debug.Log(hit.collider.gameObject.name);
            //    if (target != null)
            //    {
                    
            //        Debug.Log(target.health);
            //        target.TakeDamage(damage);
            //    }
                


            //    //GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            //    //Destroy(impactGo, 2f);
            //}
            //Debug.Log("Ga Kena!");

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
