using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeluruCannon : MonoBehaviour
{
    public GameObject hitEffect;

    private void Update()
    {
        //hitEffect.Emit(100);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Masuk OTE");
        if(other.tag == "Player")
        {
            other.GetComponent<Player>().gotHit(250);
        }
        else
        {
            if(other.name.ToLower().Contains("floor") || other.name.ToLower().Contains("street"))
            {
                //GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.GetComponent<ParticleSystem>().Play();
                Destroy(effect, 6f);
                
            }
        }
        //hitEffect.transform.position = transform.position;
        //hitEffect.transform.forward = transform.position.normalized;
        //hitEffect.Emit(100);
    }
}
