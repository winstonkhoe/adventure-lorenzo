using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Animator doorAnimator;

    bool reachTunnel;
    static bool requirements;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") || other.name.Equals("Mech"))
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
                    //CreateMessage.createMessage("NOT ENOUGH CORE ITEM");wwwwwwwwwwwwwwwwww
                }
                else
                {
                    requirements = true;
                }
            }
            if(requirements)
            {
                doorAnimator.SetBool("character_nearby", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") || other.name.Equals("Mech"))
        {
            doorAnimator.SetBool("character_nearby", false);
        }
    }

    void Start()
    {
        requirements = false;
        reachTunnel = false;
        doorAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
