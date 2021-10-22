using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingCannon : MonoBehaviour
{
    public GameObject peluruCannon;
    public Transform titikKeluarCannon;
    private float startTime, timeElapsed;
    

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed = Time.time - startTime;
        if(timeElapsed >= 3)
        {
            shootCannon();
            startTime = Time.time;
        }
    }

    void shootCannon()
    {
        GameObject o = Instantiate(peluruCannon, titikKeluarCannon.position, Quaternion.identity);
        o.transform.rotation = titikKeluarCannon.rotation;
        Rigidbody rb = o.GetComponent<Rigidbody>();
        rb.AddForce(o.transform.forward * 1000);
        //rb.AddRelativeForce(titikKeluarCannon.position, ForceMode.Impulse);
        //rb.velocity = titikKeluarCannon.position.normalized * 10;
    }
    //IEnumerator shootCannon()
    //{
        
    //}


}
