using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Setup(Vector3 arah, float bulletSpeed)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        //rigidbody.AddForce(100f)
        //rigidbody.velocity = transform.TransformDirection(new Vector3(0, 0, speed));
        rigidbody.velocity = transform.TransformDirection(arah * bulletSpeed);
        //rigidbody.AddForce(Vector3.forward * speed);
        //Debug.Log("Finished Setup");
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.GetComponent<Player>() != null)
        //{
        //    Debug.Log(other.name);
        //    other.GetComponent<Player>().gotHit(10);
        //}
    }
}
