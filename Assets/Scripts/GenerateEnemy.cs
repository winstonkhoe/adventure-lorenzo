using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenerateEnemy : MonoBehaviour
{
    public class Enemy
    {
        public GameObject enemy;
        public NavMeshAgent agent;
        public int patrolIndex;
        public Transform patrolPos;
        public bool arrived;

        public Enemy(GameObject e, NavMeshAgent a, int pI, Transform pP)
        {
            enemy = e;
            agent = a;
            patrolIndex = pI;
            patrolPos = pP;
            arrived = false;
        }

    }

    [System.Serializable]
    public class Patroli
    {
        public string enemyType;
        public GameObject enemyPrefabs;
        public Transform[] patrolPos;
        public Transform spawnPosition;
        public List<Enemy> enemyList;
        public bool[] satpamExist;
    }

    public List<Patroli> patrolPositions = new List<Patroli>();

    void Start()
    {
        foreach(Patroli p in patrolPositions)
        {
            p.satpamExist = new bool[p.patrolPos.Length];
            p.enemyList = new List<Enemy>();
        }
    }

    void Update()
    {
        foreach(Patroli p in patrolPositions)
        {
            if (p.enemyList.Count < p.patrolPos.Length)
                generateEnemy(p);
            updateEnemyArrival(p);
        }
    }

    void updateEnemyArrival(Patroli p)
    {
        foreach (Enemy e in p.enemyList)
        {
            if (e.enemy == null)
            {
                p.enemyList.Remove(e);
                break;
            }
            else
            {
                if (!e.arrived)
                {
                    if (Vector3.Distance(e.enemy.transform.position, e.patrolPos.position) < 0.5f)
                    {
                        e.arrived = true;
                        e.agent.SetDestination(e.enemy.transform.position);
                        e.enemy.GetComponent<EnemyAI>().inPosition = true;
                    }
                    else
                    {
                        //Debug.Log("Setting Destination");
                        e.agent.SetDestination(e.patrolPos.position);
                    }
                }

            }

        }
    }

    void generateEnemy(Patroli p)
    {
        for (int i = 0; i < p.satpamExist.Length; i++)
        //for (int i = 0; i < 1; i++)
        {
            if (!p.satpamExist[i])
            {
                //Debug.Log("Masuk Posisi Patrol " + i + " Kosong");
                spawnEnemy(p, i);
                p.satpamExist[i] = true;
            }
        }
    }

    void spawnEnemy(Patroli p, int patrolIndex)
    {
        //Debug.Log("Spawn Enemy for patrolIndex " + patrolIndex);
        Transform destination = p.patrolPos[patrolIndex];
        GameObject enemy = Instantiate(p.enemyPrefabs, p.spawnPosition.position, Quaternion.identity);
        enemy.GetComponent<EnemyAI>().patrolIndex = patrolIndex;
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        for (int i = 0; i < destination.transform.childCount; i++)
        {
            enemy.GetComponent<EnemyAI>().patrolPoints.Add(destination.transform.GetChild(i));
        }

        agent.transform.LookAt(destination);
        agent.SetDestination(destination.position);
        //Debug.Log("Current Enemy Position: " + enemy.transform.position);
        //Debug.Log("Patrol Point: " + destination.position);
        //Debug.Log("Agent Target: " + agent.destination);
        p.enemyList.Add(new Enemy(enemy, agent, patrolIndex, destination));
    }

    Patroli findPatroli(string enemyType)
    {
        foreach(Patroli p in patrolPositions)
        {
            if(enemyType.ToLower().Contains(p.enemyType.ToLower()))
                return p;
        }
        return null;
    }

    public void cleanPatroliExist(string enemyType, int patrolIndex, int delay)
    {
        StartCoroutine(makePatrolPointEmpty(enemyType, patrolIndex, delay));
    }

    public IEnumerator makePatrolPointEmpty(string enemyType, int patrolIndex, int delay)
    {
        Patroli p = findPatroli(enemyType);
        if(p != null)
        {
            //Debug.Log("Start Delay for " + delay) ;
            yield return new WaitForSeconds(delay);
            //Debug.Log("End Delay");
            p.satpamExist[patrolIndex] = false;
        }
        yield return null;
    }
}
