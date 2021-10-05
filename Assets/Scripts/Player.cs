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
        //Vector3 to = new Vector3(0, 90, 0);
        //radiusLine.transform.eulerAngles = Vector3.Lerp(radiusLine.transform.rotation.eulerAngles, to, Time.deltaTime);
        //Debug.Log("Player position: " + transform.position);
        //Debug.Log("Radius position: " + radiusLine.transform.position);

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

        // The MST will have V vertices
        for (int count = 0; count < V - 1; count++)
        {
            int u = minKey(key, mstSet);
            //Debug.Log("value of u: " + u);
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

        //printMST(parent, graph);
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
            //Debug.Log(g.name);
            Destroy(g);
        }
        //DestroyImmediate(g);
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
            Debug.Log("Loop: " + counter);
            counter++;
            //Debug.Log(Time.time);
            //Debug.Log(Time.deltaTime);
            timeElapsed = currentTime - startTime;
            Debug.Log(timeElapsed);
            //Debug.Log(timeElapsed);
            hitColliders = null;
            hitColliders = Physics.OverlapSphere(transform.position, radius, whatIsEnemy);
            Debug.Log("Amount of enemy: " + hitColliders.Length);
            V = hitColliders.Length;
            if (prevHitColliders != null)
            {
               
                foreach (var phc in prevHitColliders)
                {
                    //Debug.Log(phc.name);
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
                        //Debug.Log(phc.name + " will setToFalse");
                        phc.GetComponent<Target>().setInRange(false);
                    }
                }
            }
            //Debug.Log("HitCollider");
            foreach (var h in hitColliders)
            {
                //Debug.Log(h.name);
                h.GetComponent<Target>().setInRange(true);
            }

            prevHitColliders = hitColliders;
            yield return new WaitForSeconds(1);
        }
        foreach (var phc in prevHitColliders)
        {
            //Debug.Log(h.name);
            phc.GetComponent<Target>().setInRange(false);
        }

    }

    IEnumerator fadeOut(Image image, float duration)
    {
        float counter = 0;
        //Get current color
        Color spriteColor = image.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            //Fade from 1 to 0
            float alpha = Mathf.Lerp(1, 0, counter / duration);
            Debug.Log(alpha);

            //Change alpha only
            image.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            //Wait for a frame
            yield return null;
        }
    }

    void Attack()
    {
        var adjacentList = new List<List<float>>();
        foreach (var hitCollider in hitColliders)
        {
            //Debug.Log(counter + " - " + hitCollider.name);
            var adjacentListRow = new List<float>();
            foreach (var h in hitColliders)
            {
                //Debug.Log("Outer Loop: " + hitCollider.name);
                //Debug.Log("Inner Loop: " + h.name);
                float edgeWeight;
                if (hitCollider.name.Equals(h.name))
                    edgeWeight = 0;
                else
                {
                    //Debug.Log("Masuk Kondisi kedua");
                    //Debug.Log(h.name + " -" + h.transform.position.x);
                    //Debug.Log(hitCollider.name + " -" + hitCollider.transform.position.x);
                    edgeWeight = Vector3.Distance(h.transform.position, hitCollider.transform.position);
                }
                //Debug.Log(edgeWeight);
                adjacentListRow.Add(edgeWeight);
            }
            adjacentList.Add(adjacentListRow);
            //hitCollider.SendMessage("AddDamage");
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

        //for (int i = 0; i < V; i++)
        //{
        //    Debug.Log("Value Parent " + i + " " + parent[i]);
        //    //Debug.Log(i);
        //    string vertex1 = hitColliders[i].name;
        //    //Debug.Log(vertex1);
        //    string vertex2 = hitColliders[parent[i]].name;
        //    //Debug.Log(vertex2);
        //    vertexConnections[i].Add(vertex2);
        //    vertexConnections[parent[i]].Add(vertex1);
        //}
        //int j = 0;
        for (int i = V > 1 ? 1 : 0; i < V; i++)
        {
            //Debug.Log("Value Parent " + i + " " + parent[i]);
            //Debug.Log(i);
            string vertex1 = hitColliders[i].name;
            //Debug.Log(vertex1);
            string vertex2 = hitColliders[parent[i]].name;
            //Debug.Log(vertex2);
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
            //string vertex1 = hitColliders[i].name;
            //Debug.Log(vertex1 + " have " + vertexConnections[i].Count + " connected vertex :");
            //for(int j = 0; j < vertexConnections[i].Count; j++)
            //{
            //    Debug.Log("-"+vertexConnections[i][j]);
            //}
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
