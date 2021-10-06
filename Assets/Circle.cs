using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public int vertexCount = 40;
    public float lineWidth = 0.2f;
    public float radius = 4f;
    public bool circleFillscreen;

    float degrees = 90;
    
    private LineRenderer lineRenderer;
    Vector3 centerPosition;
    GameObject player;

    //Test Ellipsis
    public int segments = 40;
    public float xradius = 10;
    public float yradius = 10;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        //player = GameObject.Find("Ken");
        //centerPosition = player.transform.position;
        ////Debug.Log(centerPosition);
        //SetupCircle();
    }

    //private void SetupCircle()
    //{
    //    lineRenderer.widthMultiplier = lineWidth;
    //    float deltaTheta = (2f * Mathf.PI) / vertexCount;
    //    float theta = 0f;

    //    lineRenderer.positionCount = vertexCount;
    //    for(int i = 0; i < lineRenderer.positionCount; i++)
    //    {
    //        Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
    //        theta += deltaTheta;
    //    }
    //}
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.SetVertexCount(segments + 1);
        lineRenderer.useWorldSpace = false;
        CreatePoints();
        //BikinRadius();
    }

    //public void BikinRadius()
    //{
    //    DrawPolygon(vertexCount, radius, centerPosition, lineWidth, lineWidth);
    //}

    //void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth)
    //{
    //    lineRenderer.startWidth = startWidth;
    //    lineRenderer.endWidth = endWidth;
    //    lineRenderer.loop = true;
    //    float angle = 2 * Mathf.PI / vertexNumber;
    //    lineRenderer.positionCount = vertexNumber;

    //    for (int i = 0; i < vertexNumber; i++)
    //    {
    //        //Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
    //        //                                         new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
    //        //                           new Vector4(0, 0, 1, 0),
    //        //                           new Vector4(0, 0, 0, 1));
    //        Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
    //                                                 new Vector4(Mathf.Cos(angle * i), 1 * Mathf.Sin(angle * i), 0, 0),
    //                                                   new Vector4(0, 0, 1, 0),
    //                                                   new Vector4(0, 0, 0, 1));
    //        Vector3 initialRelativePosition = new Vector3(0, radius, 0);
    //        lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
    //    }
    //    //Vector3 to = new Vector3(degrees, 0, 0);
    //    //transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime);
    //}

    //private void OnDrawGizmos()
    //{
    //    float deltaTheta = (2f * Mathf.PI) / vertexCount;
    //    float theta = 0f;

    //    Vector3 oldPos = Vector3.zero;
    //    for(int i = 0; i < vertexCount + 1; i++)
    //    {
    //        Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
    //        Gizmos.DrawLine(oldPos, transform.position + pos);
    //        oldPos = transform.position + pos;

    //        theta += deltaTheta;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        //centerPosition = player.transform.position;
        //BikinRadius();
        //OnDrawGizmos();
        //SetupCircle();
    }

    void CreatePoints()
    {
        float x;
        float y;
        float z = 0f;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            lineRenderer.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / segments);
        }
    }

}
