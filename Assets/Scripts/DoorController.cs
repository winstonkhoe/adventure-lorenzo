using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Animator doorAnimator;
 
    bool reachTunnel;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            
            if(name.ToLower().Contains("victory"))
            {
                FindObjectOfType<AudioManager>().InterceptSong("");
            }
            if(name.ToLower().Contains("meetboss"))
            {
                if (reachTunnel == false)
                {
                    reachTunnel = true;
                    FindObjectOfType<AudioManager>().InterceptSong("Tunnel");
                }
            }
            if(name.ToLower().Contains("underground"))
            {
                Player p = other.GetComponent<Player>();
                if(p.coreItemOwned < 9)
                {
                    CreateMessage cm = FindObjectOfType<CreateMessage>();
                    cm.createMessage("NOT ENOUGH CORE ITEM");
                    //CreateMessage.createMessage("NOT ENOUGH CORE ITEM");
                }
                else
                {
                    doorAnimator.SetBool("character_nearby", true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            doorAnimator.SetBool("character_nearby", false);
        }
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
