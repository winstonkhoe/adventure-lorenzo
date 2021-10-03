using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Animator doorAnimator;
 
    bool reachTunnel;

    private void OnTriggerEnter(Collider other)
    {
        if(reachTunnel == false)
        {
            reachTunnel = true;
            FindObjectOfType<AudioManager>().InterceptSong("Tunnel");
        }
        Debug.Log("TriggerEnter");
        doorAnimator.SetBool("character_nearby", true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TriggerExit");
        doorAnimator.SetBool("character_nearby", false);
    }

    void Start()
    {
        reachTunnel = false;
        doorAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
