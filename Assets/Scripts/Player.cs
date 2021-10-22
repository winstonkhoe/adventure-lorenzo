using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
public class Player : MonoBehaviour
{
    //Time
    string fmt = "00";
    

    Vector3 playerPosition;

    public Image attackedEffect;
    

    Gun playerGun;
    

    public LayerMask whatIsEnemy;

    public DialogueTrigger dialogueTrigger;
    
    void Start()
    {
        //initGamePlay();
        playerGun = GetComponent<Gun>();
        inventory = GetComponent<Inventory>();

        initPlayer();
        dialogueTrigger.TriggerDialogue();

        startTime = Time.time;
        initializeUIMenu();
        shield.SetActive(false);
        putGunInPocket();
        initSpecialEffect();
    }

    void Update()
    {
        if(healthPoint <= 0)
        {
            playerDeath();
        }

        updateUIMenu();

        playerPosition = transform.position;

        radiusLine.transform.position = transform.position;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (checkEnemyInRange == true)
            {
                PerformSpecialEffect();
            }
            StartCoroutine(turnOnRadiusLine());
            StartCoroutine(SetBombUIEnemyInRadius(transform.position));
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            onShootingMode = !onShootingMode;
            if(onShootingMode)
            {
                ShootingMode();
            }
            else
            {
                ExplorationMode();
            }
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Gun g = GetComponent<Gun>();
            handleSwitchWeapon(g.primaryWeapon);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Gun g = GetComponent<Gun>();
            handleSwitchWeapon(g.secondaryWeapon);
        }

    }

    #region Game Play

    private void initGamePlay()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    #endregion

    #region Player

    private Transform weaponParent;
    private Transform originalGunPosition;
    public Transform rightHand;
    public Transform leftHand;
    public Transform rightPocket;
    public Transform leftPocket;

    CharacterController playerController;
    CharacterController tempController;

    Animator animator;

    public int healthPoint = 1000;
    public int skillPoint = 200;
    public int coreItemOwned = 0;
    public bool onShootingMode = false;
    private void initPlayer()
    {
        //healthPoint = 1000;
        //skillPoint = 200;
        animator = GetComponent<Animator>();
        Gun g = GetComponent<Gun>();
        playerController = GetComponent<CharacterController>();
        GameObject weapon = g.currentActiveWeapon.weaponObject;
        weaponParent = weapon.transform.parent;
        originalGunPosition = weapon.transform;
        //ExplorationMode();
    }

    private void playerDeath()
    {
        animator.SetBool("isDead", true);
        DeathCanvas();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject g = hit.gameObject;
        if (g.tag.Equals("Item"))
        {
            if (g.name.Contains("CoreItem"))
            {
                coreItemOwned++;
            }
            //Inventory
            else
            {
                inventory.AddItem(g.name);
            }
            Destroy(g);
        }
        else if (g.tag.Equals("Enemy"))
        {
            CreateMessage cm = FindObjectOfType<CreateMessage>();
            if (g.name.ToLower().Contains("mech"))
            {
                cm.createMessage("Press F to Override MECH");
                if (Input.GetKeyDown(KeyCode.F))
                {
                    gameObject.SetActive(false);
                    g.GetComponent<MechMovement>().overriden = true;
                    //CharacterController enemy = g.GetComponent<CharacterController>();
                    //GetComponent<ThirdPersonMovement>().controller = enemy;

                }
            }
        }
        else if (g.tag.Equals("Spaceship"))
        {
            CreateMessage cm = FindObjectOfType<CreateMessage>();
            cm.createMessage("Press F to Leave this world!");
            if (Input.GetKeyDown(KeyCode.F))
            {
                VictoryCanvas();
            }
        }
    }

        #region Player Animations
        
        public Rig primaryWeaponRig;
        public Rig secondaryWeaponRig;
        
        private void putGunInPocket()
        {
            Gun g = GetComponent<Gun>();
            Transform primaryWeapon = g.primaryWeapon.weaponObject.transform;
            Transform secondaryWeapon = g.secondaryWeapon.weaponObject.transform;

            primaryWeapon.parent = rightPocket;
            secondaryWeapon.parent = leftPocket;

            primaryWeapon.position = rightPocket.position;
            secondaryWeapon.position = leftPocket.position;

            primaryWeapon.rotation = rightPocket.rotation;
            secondaryWeapon.rotation = leftPocket.rotation;
        }

        private void ExplorationMode()
        {
            Gun g = GetComponent<Gun>();
            GameObject weapon = g.currentActiveWeapon.weaponObject;
            if(weapon.Equals(g.primaryWeapon.weaponObject))
            {
                StartCoroutine(SmoothRig(1, 0, primaryWeaponRig, weapon, rightHand, rightHand, rightPocket, rightPocket));
            }
            else
            {
                StartCoroutine(SmoothRig(1, 0, secondaryWeaponRig, weapon, leftHand, leftHand, leftPocket, leftPocket));
            }
            
            //weapon.transform.parent = rightPocket;
            //weapon.transform.position = rightPocket.transform.position;
            //weapon.transform.rotation = rightPocket.transform.rotation;
        }

        private void handleSwitchWeapon(Gun.Weapon w)
        {
            Gun g = GetComponent<Gun>();
            if(!g.currentActiveWeapon.Equals(w))
            {
                ExplorationMode();
                g.currentActiveWeapon = w;
                ShootingMode();
            }
        }

        private void ShootingMode()
        {
            Gun g = GetComponent<Gun>();
            GameObject weapon = g.currentActiveWeapon.weaponObject;
            //weapon.transform.parent = rightHand;
            //weapon.transform.position = rightHand.transform.position;
            if (weapon.Equals(g.primaryWeapon.weaponObject))
            {
                StartCoroutine(SmoothRig(0, 1, primaryWeaponRig, weapon, rightHand, rightHand, weaponParent, originalGunPosition));
            }
            else
            {
                StartCoroutine(SmoothRig(0, 1, secondaryWeaponRig, weapon, leftHand, leftHand, weaponParent, originalGunPosition));
            }
            //weapon.transform.position = originalGunPosition.position;
            //weapon.transform.rotation = originalGunPosition.rotation;
        }

        IEnumerator SmoothRig(float start, float end, Rig layer, GameObject obj, Transform startPosParent, Transform startPos, Transform endPosParent, Transform endPos)

        {
            float elapsedTime = 0;
            float waitTime = 0.2f;

            obj.transform.parent = startPosParent;
            obj.transform.position = startPos.transform.position;

            while (elapsedTime < waitTime)

            {
                layer.weight = Mathf.Lerp(start, end, (elapsedTime / waitTime));

                elapsedTime += Time.deltaTime;

                yield return null;

            }

            obj.transform.parent = endPosParent;
            obj.transform.position = endPos.transform.position;
            obj.transform.rotation = endPos.transform.rotation;
        }

        #endregion 

    #endregion






    #region UICanvas

    private bool isPause;
    public GameObject playerUICanvas;
    public GameObject pauseUICanvas;
    public GameObject deathUICanvas;
    public GameObject victoryUICanvas;

    public Slider healthBar;
    public Slider skillBar;

    public TMPro.TextMeshProUGUI ammoText;
    public TMPro.TextMeshProUGUI coreItemText;
    public TMPro.TextMeshProUGUI timeText;
    public TMPro.TextMeshProUGUI victoryText;

    private float startTime;
    private float elapsedTime;
    private int minute;
    private int second;

    private void initializeUIMenu()
    {
        pauseUICanvas.SetActive(false);
        deathUICanvas.SetActive(false);
        isPause = false;
        healthBar.maxValue = healthPoint;
        skillBar.maxValue = skillPoint;
    }

    private void updateUIMenu()
    {
        //elapsedTime = Time.time - startTime;
        //second = (int)elapsedTime % 60;
        //minute = (int)((elapsedTime - second) / 60);
        //timeText.text = minute.ToString(fmt) + ":" + second.ToString(fmt);

        ammoText.text = playerGun.AmmoText();
        //ammoText.text = "30";
        coreItemText.text = "CORE ITEM: " + coreItemOwned.ToString(fmt) + "/09";
        healthBar.value = healthPoint;
        skillBar.value = skillPoint;
    }

    private void PauseGame()
    {
        if (!isPause)
        {
            isPause = true;
            pauseUICanvas.SetActive(true);
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void DeathCanvas()
    {
        playerUICanvas.SetActive(false);
        deathUICanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    }

    public void ResumeGame()
    {
        pauseUICanvas.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void VictoryCanvas()
    {
        playerUICanvas.SetActive(false);
        victoryText.text = "Finished In " + minute.ToString(fmt) + ":" + second.ToString(fmt);
        victoryUICanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        FindObjectOfType<AudioManager>().clearSong();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    #endregion








    #region Special Effect - Prim's Algorithm

    //PRIM
    List<GameObject> vertex;
    Collider[] hitColliders, prevHitColliders;
    private static int V = 5;
    static int[] parent;
    
    //Effect
    public GameObject lightningEffect;

    //Radius Visualization
    public LineRenderer radiusLine;
    public int segments = 40;
    public float xradius = 10f;
    public float yradius = 10f;
    public float radius = 10f;
    private bool checkEnemyInRange = false;

    //Damage
    public int electricDamage = 125;

    #region Special Effect Radius

    void initSpecialEffect()
        {
            vertex = new List<GameObject>();
            initRadius();
        }

        void initRadius()
        {
            radiusLine.positionCount = segments + 1;
            //radiusLine.SetVertexCount(segments + 1);
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

        IEnumerator turnOnRadiusLine()
        {
            checkEnemyInRange = true;
            radiusLine.gameObject.SetActive(true);

            yield return new WaitForSeconds(5);

            radiusLine.gameObject.SetActive(false);
            checkEnemyInRange = false;
        }

        IEnumerator SetBombUIEnemyInRadius(Vector3 center)
        {
            float startTime = Time.time;
            float timeElapsed = 0;
            while (timeElapsed <= 5)
            {
                timeElapsed = Time.time - startTime;
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
                            if (phc.Equals(hc))
                            {
                                flagExist = true;
                                break;
                            }
                        }
                        if (!flagExist)
                        {
                            phc.GetComponent<EnemyAI>().setInRange(false);
                        }
                    }
                }
                foreach (var h in hitColliders)
                {
                    h.GetComponent<EnemyAI>().setInRange(true);
                }

                prevHitColliders = hitColliders;
                yield return new WaitForSeconds(0);
            }
            foreach (var phc in prevHitColliders)
            {
                phc.GetComponent<EnemyAI>().setInRange(false);
            }
        }
        #endregion

        #region Special Effect Algorithm

        static int minKey(float[] key, bool[] mstSet)
        {
            // Initialize min value
            float min = float.MaxValue;
            int min_index = -1;

            for (int v = 0; v < V; v++)
            {
                if (mstSet[v] == false && key[v] < min)
                {
                    min = key[v];
                    min_index = v;
                }
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
                //Debug.Log(u);
                //Debug.Log(mstSet[u]);
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

        void GiveElectricDamageAndLightningStrike()
        {
            List<List<string>> vertexConnections = new List<List<string>>(V);
            for (int i = 0; i < V; i++)
            {
                vertexConnections.Add(new List<string>());
            }

            for (int i = V > 1 ? 1 : 0; i < V; i++)
            {
                string vertex1 = vertex[i].name;
                string vertex2 = vertex[parent[i]].name;
                vertexConnections[i].Add(vertex2);


                GameObject lightning = Instantiate(lightningEffect, vertex[parent[i]].transform.position, Quaternion.identity);
                DigitalRuby.LightningBolt.LightningBoltScript lightningScript = lightning.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
                lightningScript.StartObject = vertex[parent[i]].gameObject;
                lightningScript.EndObject = vertex[i].gameObject;
                //lightningScript.Duration = 5f;
                Destroy(lightning, 6f);

                if (V > 1) //Kalau Single Vertex gaperlu add parent karena diri dia sendiri
                {
                    vertexConnections[parent[i]].Add(vertex1);
                }
            }
            for (int i = 0; i < V; i++)
            {
                if (vertex[i].tag != "Player")
                {
                EnemyAI t = vertex[i].GetComponent<EnemyAI>();
                    t.TakeDamage(electricDamage * vertexConnections[i].Count);
                }
            }
            skillPoint -= 75;
        }
        void PerformSpecialEffect()
        {
            if (vertex.Count > 0)
                vertex.Clear();

            var adjacentList = new List<List<float>>();
            makeAdjacencyList(adjacentList);
            primMST(adjacentList);
            GiveElectricDamageAndLightningStrike();
        }

        void makeAdjacencyList(List<List<float>> adjacentList)
        {
            hitColliders = Physics.OverlapSphere(transform.position, radius, whatIsEnemy);

            foreach (Collider c in hitColliders)
            {
                vertex.Add(c.gameObject);
            }
            vertex.Add(gameObject);

            V = vertex.Count;

            foreach (var i in vertex)
            {
                var adjacentListRow = new List<float>();
                foreach (var j in vertex)
                {
                    float edgeWeight;
                    if (i.Equals(j))
                        edgeWeight = 0;
                    else
                        edgeWeight = Vector3.Distance(j.transform.position, i.transform.position);
                    adjacentListRow.Add(edgeWeight);
                }
                adjacentList.Add(adjacentListRow);
            }
        }

        #endregion


    #endregion



    #region Inventory

    Inventory inventory;
    public GameObject shield;

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

        if (healthPoint >= prevHealth)
        {
            healthPoint = prevHealth;
        }
    }

    IEnumerator applyShield()
    {
        shield.SetActive(true);

        yield return new WaitForSeconds(7);

        shield.SetActive(false);
    }

    IEnumerator applyDamageMultiplier()
    {
        playerGun.damageMultiplier *= 2;

        yield return new WaitForSeconds(5);

        playerGun.damageMultiplier /= 2;
    }

    #endregion

    
    public void gotHit(int damage)
    {
        healthPoint -= damage;
        Color spriteColor = attackedEffect.color;
        attackedEffect.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 100);
        StartCoroutine(fadeOut(attackedEffect, 5f));
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

    

    
}
