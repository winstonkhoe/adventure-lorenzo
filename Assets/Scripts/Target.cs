using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    Animator animator;
    CharacterController controller;
    public GameObject coreItem;

    private bool droppedCoreItem = false;
    public float health = 50f;
<<<<<<< HEAD
=======
    private bool inRange = false;
>>>>>>> f6d28fc (Upload SLN(2))
    public void TakeDamage(float amount)
    {
        health -= amount;
        if(health <= 0f)
        {
            Die();
        }
    }
    void Die()
    {
        animator.SetBool("isDead", true);
        controller.Move(new Vector3(0, -5f).normalized * Time.deltaTime);
        if(gameObject.tag.Equals("Enemy") && droppedCoreItem == false)
        {
            droppedCoreItem = true;
            Debug.Log("Keluarin Core Item");
            //Instantiate(coreItem, new Vector3(controller.transform.position.x, controller.transform.position.y, controller.transform.position.z), Quaternion.identity);
            Instantiate(coreItem, controller.transform.position, Quaternion.identity);
            //Instantiate(coreItem, controller.transform, true);
        }
        Destroy(gameObject, 4f);
<<<<<<< HEAD

    }
=======
    }

    public void setInRange(bool value)
    {
        inRange = value;
    }

    public bool getInRange()
    {
        return inRange;
    }

>>>>>>> f6d28fc (Upload SLN(2))
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
