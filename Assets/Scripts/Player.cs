using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Time
    float currentTime;

    //Special Skill
    //Radius
    public int segments = 40;
    public float xradius = 10f;
    public float yradius = 10f;

    private bool checkEnemyInRange = false;
    Vector3 playerPosition;

    //Attack  
    Collider[] hitColliders;
    Collider[] prevHitColliders;
    public Image attackedEffect;
    public float electricDamage = 125f;

    //Bar Values
    public int healthPoint = 1000;
    private int extraHealth = 0;
    public int skillPoint = 200;
    public Slider healthBar;
    public Slider skillBar;

    Gun playerGun;
    public LineRenderer radiusLine;

    public LayerMask whatIsEnemy;

    //Ammo

    public TMPro.TextMeshProUGUI ammoText;

    //CoreItem
    public TMPro.TextMeshProUGUI coreItemText;
    private int coreItemOwned = 0;

    //Inventory
    Inventory inventory;
    public GameObject shield;

    private static int V = 5;
    static int[] parent;


    
    void Start()
    {
        inventory = GetComponent<Inventory>();
        playerGun = GetComponent<Gun>();
        shield.SetActive(false);
        healthBar.maxValue = healthPoint;
        skillBar.maxValue = skillPoint;
        initRadius();
    }

    void Update()
    {
        currentTime = Time.time;
        ammoText.text = playerGun.AmmoText();
        coreItemText.text = "CORE ITEM: 0" + coreItemOwned + "/09";
        healthBar.value = healthPoint;
        skillBar.value = skillPoint;
        playerPosition = transform.position;

        radiusLine.transform.position = transform.position;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //Debug.Log("Z Kepencet");
            //radiusLine.b
            if (checkEnemyInRange == true)
            {
                Attack();
            }
            StartCoroutine(turnOnRadiusLine());
            StartCoroutine(ThunderEffectCheckEnemy(transform.position, 10f));
        }
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.UseItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.UseItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.UseItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.UseItem(4);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            inventory.UseItem(5);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            inventory.UseItem(6);
        }
    }

    static int minKey(float[] key, bool[] mstSet)
    {

        // Initialize min value
        float min = float.MaxValue;
        int min_index = -1;

        for (int v = 0; v < V; v++)
            if (mstSet[v] == false && key[v] < min)
            {
                min = key[v];
                min_index = v;
            }

        return min_index;
    }

    static void primMST(List<List<float>> graph)
    {

        parent = new int[V];

        float[] key = new float[V];

        bool[] mstSet = new bool[V];

        for (int i = 0; i < V; i++)
        {
            key[i] = float.MaxValue;
            mstSet[i] = false;
        }

        key[0] = 0;
        parent[0] = -1;

        if (V < 2)
        {
            parent[0] = 0;
        }

        for (int count = 0; count < V - 1; count++)
        {
            int u = minKey(key, mstSet);
            mstSet[u] = true;

            for (int v = 0; v < V; v++)
            {
                if (((List<float>)graph[u])[v] != 0 && mstSet[v] == false
                    && ((List<float>)graph[u])[v] < key[v])
                {
                    parent[v] = u;
                    key[v] = ((List<float>)graph[u])[v];
                }
            }
        }
    }

    void initRadius()
    {
        radiusLine.SetVertexCount(segments + 1);
        radiusLine.useWorldSpace = false;
        CreatePoints();
        radiusLine.gameObject.SetActive(false);
    }

    void CreatePoints()
    {
        float x;
        float y = 0f;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            radiusLine.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }

    public void useAmmo()
    {
        playerGun.IncreaseAmmo(30);
    }

    public void useHealthPotion()
    {
        healthPoint += 200;
    }

    public void useSkillPotion()
    {
        skillPoint += 75;
    }

    public void useShield()
    {
        StartCoroutine(applyShield());
    }

    public void usePainKiller()
    {
        StartCoroutine(applyPainKiller());
    }

    public void useDamageMultiplier()
    {
        StartCoroutine(applyDamageMultiplier());
    }

    IEnumerator applyPainKiller()
    {
        int prevHealth = healthPoint;
        healthPoint += 450;
        
        yield return new WaitForSeconds(1);
        healthPoint -= 90;

        yield return new WaitForSeconds(1);
        healthPoint -= 90;

        yield return new WaitForSeconds(1);
        healthPoint -= 90;

        yield return new WaitForSeconds(1);
        healthPoint -= 90;
        
        yield return new WaitForSeconds(1);
        healthPoint -= 90;
        
        if(healthPoint >= prevHealth)
        {
            healthPoint = prevHealth;
        }
    }

    IEnumerator applyShield()
    {
        shield.SetActive(true);

        yield return new WaitForSeconds(5);

        shield.SetActive(false);
    }

    IEnumerator applyDamageMultiplier()
    {
        playerGun.damageMultiplier *= 2;

        yield return new WaitForSeconds(5);

        playerGun.damageMultiplier /= 2;
    }

    public void gotHit(int damage)
    {
        healthPoint -= damage;
        Color spriteColor = attackedEffect.color;
        attackedEffect.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 100);
        StartCoroutine(fadeOut(attackedEffect, 5f));
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject g = hit.gameObject;
        if (g.tag.Equals("Item"))
        {
            if (g.name.Contains("CoreItem"))
            {
                //Debug.Log("Masuk");
                coreItemOwned++;
            }
            //Inventory
            else
            {
                inventory.AddItem(g.name);
            }
            Destroy(g);
        }
    }

    void checkRadius()
    {
        ThunderEffectCheckEnemy(playerPosition, 10f);
    }

    IEnumerator ThunderEffectCheckEnemy(Vector3 center, float radius)
    {
        float startTime = currentTime;
        float timeElapsed = 0;
        int counter = 0;
        while (timeElapsed <= 5)
        {
            counter++;
            //Debug.Log(Time.time);
            //Debug.Log(Time.deltaTime);
            timeElapsed = currentTime - startTime;
            hitColliders = null;
            hitColliders = Physics.OverlapSphere(transform.position, radius, whatIsEnemy);
            V = hitColliders.Length;
            if (prevHitColliders != null)
            {
               
                foreach (var phc in prevHitColliders)
                {
                    bool flagExist = false;
                    foreach (var hc in hitColliders)
                    {
                        if (phc.name.Equals(hc.name))
                        {
                            flagExist = true;
                            break;
                        }
                    }
                    if (!flagExist)
                    {
                        phc.GetComponent<Target>().setInRange(false);
                    }
                }
            }
            foreach (var h in hitColliders)
            {
                h.GetComponent<Target>().setInRange(true);
            }

            prevHitColliders = hitColliders;
            yield return new WaitForSeconds(1);
        }
        foreach (var phc in prevHitColliders)
        {
            phc.GetComponent<Target>().setInRange(false);
        }

    }

    IEnumerator fadeOut(Image image, float duration)
    {
        float counter = 0;
        Color spriteColor = image.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);

            image.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            yield return null;
        }
    }

    void Attack()
    {
        var adjacentList = new List<List<float>>();
        foreach (var hitCollider in hitColliders)
        {
            var adjacentListRow = new List<float>();
            foreach (var h in hitColliders)
            {
                float edgeWeight;
                if (hitCollider.name.Equals(h.name))
                    edgeWeight = 0;
                else
                {
                    edgeWeight = Vector3.Distance(h.transform.position, hitCollider.transform.position);
                }
                adjacentListRow.Add(edgeWeight);
            }
            adjacentList.Add(adjacentListRow);
        }
        primMST(adjacentList);
        GiveElectricDamage();
    }

    void GiveElectricDamage()
    {
        List<List<string>> vertexConnections = new List<List<string>>(V);
        for (int i = 0; i < V; i++)
        {
            vertexConnections.Add(new List<string>());
        }

        for (int i = V > 1 ? 1 : 0; i < V; i++)
        {
            string vertex1 = hitColliders[i].name;
            string vertex2 = hitColliders[parent[i]].name;
            vertexConnections[i].Add(vertex2);
            
            
            if(V > 1) //Kalau Single Vertex gaperlu add parent karena diri dia sendiri
            {
                vertexConnections[parent[i]].Add(vertex1);
            }
        }
        for (int i = 0; i < V; i++)
        {
            Target t = hitColliders[i].GetComponent<Target>();
            t.TakeDamage(electricDamage * vertexConnections[i].Count);
        }
        skillPoint -= 75;

    }

    IEnumerator turnOnRadiusLine()
    {
        checkEnemyInRange = true;
        radiusLine.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);

        radiusLine.gameObject.SetActive(false);
        checkEnemyInRange = false;
    }
}
