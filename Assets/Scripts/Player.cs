using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Collider[] hitColliders;
    // Number of vertices in the graph
    private static int V = 5;
    static int[] parent;
    // A utility function to find
    // the vertex with minimum key
    // value, from the set of vertices
    // not yet included in MST
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

    // A utility function to print
    // the constructed MST stored in
    // parent[]
    static void printMST(int[] parent, List<List<float>> graph)
    {
        Debug.Log("Edge \tWeight");
        for (int i = 1; i < V; i++)
            Debug.Log(parent[i] + " - " + i + "\t" + graph[i][parent[i]]);
    }

        // Function to construct and
        // print MST for a graph represented
        // using adjacency matrix representation
        static void primMST(List<List<float>> graph)
        {

            // Array to store constructed MST
            parent = new int[V];

            // Key values used to pick
            // minimum weight edge in cut
            float[] key = new float[V];

            // To represent set of vertices
            // included in MST
            bool[] mstSet = new bool[V];

            // Initialize all keys
            // as INFINITE
            for (int i = 0; i < V; i++)
            {
                key[i] = float.MaxValue;
                mstSet[i] = false;
            }

            // Always include first 1st vertex in MST.
            // Make key 0 so that this vertex is
            // picked as first vertex
            // First node is always root of MST
            key[0] = 0;
            parent[0] = -1;

            // The MST will have V vertices
            for (int count = 0; count < V - 1; count++)
            {

                // Pick thd minimum key vertex
                // from the set of vertices
                // not yet included in MST
                int u = minKey(key, mstSet);
            Debug.Log("value of u: " + u);
                // Add the picked vertex
                // to the MST Set
                mstSet[u] = true;

            // Update key value and parent
            // index of the adjacent vertices
            // of the picked vertex. Consider
            // only those vertices which are
            // not yet included in MST
            for (int v = 0; v < V; v++)

                // graph[u][v] is non zero only
                // for adjacent vertices of m
                // mstSet[v] is false for vertices
                // not yet included in MST Update
                // the key only if graph[u][v] is
                // smaller than key[v]
                    if (((List<float>)graph[u])[v] != 0 && mstSet[v] == false
                        && ((List<float>)graph[u])[v] < key[v])
                    {
                        parent[v] = u;
                        key[v] = ((List<float>)graph[u])[v];
                    }
            }

            // print the constructed MST
            printMST(parent, graph);
        }
    public int segments = 40;
    public float xradius = 10f;
    public float yradius = 10f;

    public int healthPoint = 1000;
    public int skillPoint = 200;
    public Slider healthBar;

    public Gun playerGun;
    public TMPro.TextMeshProUGUI ammoText;
    public LineRenderer radiusLine;

    public LayerMask whatIsEnemy;

    void Start()
    {
        playerGun = transform.GetComponent<Gun>();
        radiusLine.SetVertexCount (segments + 1);
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

    void useHealthPotion()
    {
        healthPoint += 200;
    }

    public void gotHit(int damage)
    {
        healthPoint -= damage;
    }

    // Update is called once per frame
    void Update()
    {
        ammoText.text = playerGun.AmmoText();
        healthBar.value = healthPoint;
        
        radiusLine.transform.position = transform.position;
        //Vector3 to = new Vector3(0, 90, 0);
        //radiusLine.transform.eulerAngles = Vector3.Lerp(radiusLine.transform.rotation.eulerAngles, to, Time.deltaTime);
        //Debug.Log("Player position: " + transform.position);
        //Debug.Log("Radius position: " + radiusLine.transform.position);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Z Kepencet");
            //radiusLine.b
            StartCoroutine(turnOnRadiusLine());
            ExplosionDamage(transform.position, 10f);
        }
    }

    void ExplosionDamage(Vector3 center, float radius)
    {
        hitColliders = Physics.OverlapSphere(center, radius, whatIsEnemy);
        V = hitColliders.Length;
        var adjacentList = new List<List<float>>();

        int counter = 0;
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(counter + " - " + hitCollider.name);
            counter++;
            var adjacentListRow = new List<float>();
            foreach (var h in hitColliders)
            {
                Debug.Log("Outer Loop: " + hitCollider.name);
                Debug.Log("Inner Loop: " + h.name);
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
                Debug.Log(edgeWeight);
                adjacentListRow.Add(edgeWeight);
            }
            adjacentList.Add(adjacentListRow);
            //hitCollider.SendMessage("AddDamage");
        }
        Debug.Log(adjacentList[0][0]+","+ adjacentList[0][1]+","+ adjacentList[0][2]);
        Debug.Log(adjacentList[1][0]+","+ adjacentList[1][1]+","+ adjacentList[1][2]);
        Debug.Log(adjacentList[2][0] + "," + adjacentList[2][1] + "," + adjacentList[2][2]);
        primMST(adjacentList);
        getConnectedVertices();
    }

    void getConnectedVertices()
    {
        List<List<string>> vertexConnections = new List<List<string>>(V);
        for(int i = 0; i < V; i++)
        {
            vertexConnections.Add(new List<string>());
        }
        for (int i = 1; i < V; i++)
        {
            Debug.Log(i);
            string vertex1 = hitColliders[i].name;
            Debug.Log(vertex1);
            string vertex2 = hitColliders[parent[i]].name;
            Debug.Log(vertex2);
            vertexConnections[i].Add(vertex2);
            vertexConnections[parent[i]].Add(vertex1);
        }
        for (int i = 1; i < V; i++)
        {
            string vertex1 = hitColliders[i].name;
            Debug.Log(vertex1 + " have " + vertexConnections[i].Count + " connected vertex :");
            for(int j = 0; j < vertexConnections[i].Count; j++)
            {
                Debug.Log("-"+vertexConnections[i][j]);
            }
        }

    }

    IEnumerator turnOnRadiusLine()
    {
        radiusLine.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);

        radiusLine.gameObject.SetActive(false);
    }
}
